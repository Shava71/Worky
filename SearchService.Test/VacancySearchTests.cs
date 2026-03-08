using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SearchService.Contract;
using SearchService.DAL.Dto;
using SearchService.DAL.Entities;

namespace SearchService.Test;

public class VacancySearchTests
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions;

    public VacancySearchTests()
    {
        // Создаем HttpClient с базовым URL вашего сервиса
        _client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5006") // Укажите ваш порт
        };
            
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
    
    [Fact(DisplayName = "TC-VS-001: Поиск вакансий по ключевому слову возвращает релевантные результаты")]
    public async Task SearchVacancies_ByKeyword_ReturnsRelevantResults()
    {
        // Arrange
        var keyword = "методика";
        var url = $"/api/vacancies?AISearch={Uri.EscapeDataString(keyword)}&Page=1&PageSize=20";

        // Act
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.True(content.Total > 0, "Должны найтись вакансии по ключевому слову");
        Assert.NotNull(content.Items);
        Assert.NotEmpty(content.Items);
            
        // Проверяем структуру каждого элемента
        foreach (var item in content.Items)
        {
            Assert.NotNull(item);
            Assert.NotEqual(Guid.Empty, item.Document.id);
            Assert.NotNull(item.Document.post);
                
            // Проверка вложенного объекта Company
            Assert.NotNull(item.Document.company);
            Assert.NotNull(item.Document.company.name);
        }

        string scopeKeyword = "метод";
        // Проверяем, что хотя бы одна вакансия содержит ключевое слово
        var hasKeyword = content.Items.Any(item => 
            item.Document.post.Contains(scopeKeyword, StringComparison.OrdinalIgnoreCase) ||
            (item.Document.description?.Contains(scopeKeyword, StringComparison.OrdinalIgnoreCase) ?? false));
            
        Assert.True(hasKeyword, $"Хотя бы одна вакансия должна содержать слово '{scopeKeyword}'");
    }
    
    [Fact(DisplayName = "TC-VS-002: Фильтрация вакансий по диапазону зарплаты")]
    public async Task SearchVacancies_WithSalaryRange_FiltersCorrectly()
    {
        // Arrange
        var minSalary = 40000;
        var maxSalary = 60000;
        var url = $"/api/vacancies?AISearch=методика&min_wantedSalary={minSalary}&max_wantedSalary={maxSalary}&Page=1&PageSize=20";

        // Act
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);

        // Проверяем, что все вакансии попадают в диапазон зарплат
        foreach (var item in content.Items)
        {
         
            Assert.True(item.Document.minSalary >= minSalary, $"MinSalary {item.Document.minSalary} должен быть >= {minSalary}");
            Assert.True(item.Document.maxSalary <= maxSalary, $"maxSalary {item.Document.maxSalary} должен быть <= {maxSalary}");

            
        }
    }
    
    [Fact(DisplayName = "TC-VS-003: Фильтрация вакансий по уровню образования")]
    public async Task SearchVacancies_ByEducation_FiltersCorrectly()
    {
        // Arrange
        var educationId = 4; // Среднее профессиональное образование
        var url = $"/api/vacancies?AISearch=тренер&education={educationId}&Page=1&PageSize=20";

        // Act
        var response = await _client.GetAsync(url);
        var content = await response.Content.ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);

        // Проверяем, что все вакансии имеют указанный educationId
        foreach (var item in content.Items)
        {
            Assert.Equal(educationId, item.Document.educationId);
            Assert.NotNull(item.Document.educationName);
        }
    }
    
    [Fact(DisplayName = "TC-VS-004: Передача невалидного значения education возвращает 400 Bad Request")]
    public async Task SearchVacancies_WithInvalidEducation_ReturnsBadRequest()
    {
        // Arrange
        var invalidEducation = 9; // Вне допустимого диапазона 1-8
        var url = $"/api/vacancies?education={invalidEducation}&Page=1&PageSize=20";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            
        // Проверяем, что в ответе есть сообщение об ошибке валидации
        var errorContent = await response.Content.ReadAsStringAsync();
        Assert.Contains("education", errorContent, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("1", errorContent);
        Assert.Contains("8", errorContent);
    }
    
    [Fact(DisplayName = "TC-VS-005: Корректная работа пагинации")]
    public async Task SearchVacancies_Pagination_WorksCorrectly()
    {
        // Arrange
        var keyword = "тренер";
            
        // Act - Page 1, Size 10
        var url1 = $"/api/vacancies?AISearch={keyword}&Page=1&PageSize=10";
        var response1 = await _client.GetAsync(url1);
        var content1 = await response1.Content.ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);
            
        // Act - Page 2, Size 10
        var url2 = $"/api/vacancies?AISearch={keyword}&Page=2&PageSize=10";
        var response2 = await _client.GetAsync(url2);
        var content2 = await response2.Content.ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);
            
        // Act - Page 1, Size 20
        var url3 = $"/api/vacancies?AISearch={keyword}&Page=1&PageSize=20";
        var response3 = await _client.GetAsync(url3);
        var content3 = await response3.Content.ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response1.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response2.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response3.StatusCode);
            
        // Проверяем размеры страниц
        Assert.Equal(10, content1.Items.Count);
        Assert.Equal(1, content1.Page);
        Assert.Equal(10, content1.PageSize);
                
        Assert.Equal(10, content2.Items.Count);
        Assert.Equal(2, content2.Page);
        Assert.Equal(10, content2.PageSize);
        
        Assert.Equal(20, content3.Items.Count);
        Assert.Equal(1, content3.Page);
        Assert.Equal(20, content3.PageSize);
                
        // Проверяем, что данные на разных страницах разные
        if (content1.Items.Count > 0 && content2.Items.Count > 0)
        {
            var ids1 = content1.Items.Select(i => i.Document.id).ToHashSet();
            var ids2 = content2.Items.Select(i => i.Document.id).ToHashSet();
                    
            // Пересечение множеств ID должно быть пустым (разные вакансии)
            Assert.Empty(ids1.Intersect(ids2));
        }
    }
}