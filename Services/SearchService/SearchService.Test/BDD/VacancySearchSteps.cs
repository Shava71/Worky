using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using SearchService.Contract;
using SearchService.DAL.Dto;

namespace SearchService.Test.BDD.StepDefinitions;

[Binding]
public class VacancySearchSteps
{
    private static readonly HttpClient _client = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5006")
    };
    private HttpResponseMessage _response;
    private SearchResponse<VacancySearchResultDto> _searchResult;

    private readonly JsonSerializerOptions _jsonOptions;

    public VacancySearchSteps()
    {
        // _client = new HttpClient
        // {
        //     BaseAddress = new Uri("http://localhost:5006")
        // };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    // -----------------------------
    // VacancySearch.feature
    // -----------------------------

    [Given(@"сервис поиска вакансий запущен")]
    public void GivenServiceIsRunning()
    {
        _client.Should().NotBeNull();
    }

    [Given(@"в системе существуют вакансии содержащие слово ""(.*)""")]
    public async void GivenVacanciesWithKeyword(string keyword)
    {
        var response = await _client.GetAsync($"/api/vacancies?AISearch={keyword}&Page=1&PageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [When(@"пользователь выполняет поиск вакансий по ключевому слову ""(.*)""")]
    public async Task WhenUserSearchesByKeyword(string keyword)
    {
        _response = await _client.GetAsync($"/api/vacancies?AISearch={keyword}&Page=1&PageSize=20");

        _searchResult = await _response.Content
            .ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);
    }

    [Then(@"система должна вернуть список вакансий")]
    public void ThenSystemReturnsVacancies()
    {
        _response.StatusCode.Should().Be(HttpStatusCode.OK);
        _searchResult.Should().NotBeNull();
    }

    [Then(@"список вакансий не должен быть пустым")]
    public void ThenVacancyListShouldNotBeEmpty()
    {
        _searchResult.Items.Should().NotBeEmpty();
    }

    [Then(@"хотя бы одна вакансия должна содержать слово ""(.*)""")]
    public void ThenAtLeastOneVacancyContains(string keyword)
    {
        var found = _searchResult.Items.Any(v =>
            v.Document.post.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            (v.Document.description?.Contains(keyword, StringComparison.OrdinalIgnoreCase) ?? false));

        found.Should().BeTrue();
    }

    // -----------------------------
    // VacancySalaryFilter.feature
    // -----------------------------

    [When(@"пользователь ищет вакансии с зарплатой от (.*) до (.*)")]
    public async Task WhenUserFiltersBySalary(int minSalary, int maxSalary)
    {
        _response = await _client.GetAsync(
            $"/api/vacancies?AISearch=методика&min_wantedSalary={minSalary}&max_wantedSalary={maxSalary}&Page=1&PageSize=20");

        _searchResult = await _response.Content
            .ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);
    }

    [Then(@"минимальная зарплата каждой вакансии должна быть не меньше (.*)")]
    public void ThenMinSalaryShouldBeGreaterOrEqual(int minSalary)
    {
        foreach (var item in _searchResult.Items)
        {
            item.Document.minSalary.Should().BeGreaterThanOrEqualTo(minSalary);
        }
    }

    [Then(@"максимальная зарплата каждой вакансии должна быть не больше (.*)")]
    public void ThenMaxSalaryShouldBeLessOrEqual(int maxSalary)
    {
        foreach (var item in _searchResult.Items)
        {
            item.Document.maxSalary.Should().BeLessThanOrEqualTo(maxSalary);
        }
    }

    // -----------------------------
    // VacancyEducationFilter.feature
    // -----------------------------

    [When(@"пользователь ищет вакансии с уровнем образования ""(.*)""")]
    public async Task WhenUserFiltersByEducation(int educationId)
    {
        _response = await _client.GetAsync(
            $"/api/vacancies?AISearch=тренер&education={educationId}&Page=1&PageSize=20");

        _searchResult = await _response.Content
            .ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);
    }

    [Then(@"каждая вакансия должна иметь educationId равный (.*)")]
    public void ThenEachVacancyHasEducation(int educationId)
    {
        foreach (var item in _searchResult.Items)
        {
            item.Document.educationId.Should().Be(educationId);
        }
    }

    // -----------------------------
    // VacancyValidation.feature
    // -----------------------------

    [When(@"пользователь выполняет поиск вакансий с параметром education ""(.*)""")]
    public async Task WhenUserSearchesWithInvalidEducation(int education)
    {
        _response = await _client.GetAsync(
            $"/api/vacancies?education={education}&Page=1&PageSize=20");
    }

    [Then(@"система должна вернуть статус ошибки (.*)")]
    public void ThenSystemReturnsStatus(int statusCode)
    {
        ((int)_response.StatusCode).Should().Be(statusCode);
    }

    [Then(@"сообщение должно содержать текст ""(.*)""")]
    public async Task ThenErrorMessageContains(string text)
    {
        var content = await _response.Content.ReadAsStringAsync();

        content.Should().Contain(text);
    }

    // -----------------------------
    // VacancyPagination.feature
    // -----------------------------

    private SearchResponse<VacancySearchResultDto> _page1;
    private SearchResponse<VacancySearchResultDto> _page2;

    [When(@"пользователь ищет вакансии по слову ""(.*)"" на странице (.*) с размером страницы (.*)")]
    public async Task WhenUserRequestsPage(string keyword, int page, int size)
    {
        var response = await _client.GetAsync(
            $"/api/vacancies?AISearch={keyword}&Page={page}&PageSize={size}");

        var content = await response.Content
            .ReadFromJsonAsync<SearchResponse<VacancySearchResultDto>>(_jsonOptions);

        if (page == 1)
            _page1 = content;
        else
            _page2 = content;
    }

    [Then(@"результаты первой и второй страницы должны отличаться")]
    public void ThenPagesShouldBeDifferent()
    {
        var ids1 = _page1.Items.Select(i => i.Document.id).ToHashSet();
        var ids2 = _page2.Items.Select(i => i.Document.id).ToHashSet();

        ids1.Intersect(ids2).Should().BeEmpty();
    }
}