import React, { useEffect, useState } from 'react';
import {
    Container,
    Paper,
    Typography,
    Box,
    Stack,
    Button,
    Divider,
    Snackbar,
    Alert,
    Grid,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Chip,
    Slider,
    CircularProgress,
    TextField, ButtonGroup,
} from '@mui/material';
import dayjs from 'dayjs';
import qs from "qs";
import api from '../api/axios';
import { API } from '../api/routes';

export default function ResumesPage() {
    const userRole = localStorage.getItem('role');
    const [resumes, setResumes] = useState([]);
    const [vacancies,  setVacancies] = useState([]);
    const [filters, setFilters] = useState({
        city: '',
        min_experience: 0,
        max_experience: 100,
        education: '',
        min_wantedSalary: 0,
        max_wantedSalary: 1000000,
        type: '',
        direction: [],
        sortItem: '',
        order: 'asc',
        AISearch: '',
        Page: "1",
        PageSize: "20"
    });
    const [appliedFilters, setAppliedFilters] = useState(filters); // <-- для запроса резюме
    const [selectedVacancyId, setSelectedVacancyId] = useState({});
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [availableFilters, setAvailableFilters] = useState([]);
    const [educationList, setEducationList] = useState([]);
    const [sessionId, setSessionId] = useState(null);

    // Загрузка вакансий, фильтров и образования один раз
    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            await Promise.all([fetchVacancies(), fetchAvailableFilters(), fetchEducation()]);
            setLoading(false);
        };
        fetchData();
    }, []);

    // Загрузка резюме при appliedFilters
    useEffect(() => {
        const fetchResumes = async () => {
            setLoading(true);
            try {
                const response = await api.get(API.search.resumes, {
                    params: {
                        city: appliedFilters.city || undefined,
                        min_experience: appliedFilters.min_experience || undefined,
                        max_experience: appliedFilters.max_experience || undefined,
                        education: appliedFilters.education || undefined,
                        min_wantedSalary: appliedFilters.min_wantedSalary || undefined,
                        max_wantedSalary: appliedFilters.max_wantedSalary || undefined,
                        type: appliedFilters.type || undefined,
                        direction: appliedFilters.direction.length ? appliedFilters.direction : undefined,
                        SortItem: appliedFilters.sortItem || undefined,
                        Order: appliedFilters.order || undefined,
                        AISearch: appliedFilters.AISearch || undefined,
                        Page: appliedFilters.Page || undefined,
                        PageSize: appliedFilters.PageSize || undefined,
                    },
                    paramsSerializer: (params) => qs.stringify(params, { arrayFormat: 'repeat' }),
                });
                setResumes(response.data.items.map(x => x.document) || []);
                setSessionId(response.data.sessionId);
            } catch (err) {
                console.error('Ошибка при загрузке резюме:', err);
                showSnackbar('Не удалось загрузить список резюме', 'error');
            } finally {
                setLoading(false);
            }
        };
        fetchResumes();
    }, [appliedFilters]);

    const fetchVacancies = async () => {
        try {
            const res = await api.get(API.company.myVacancy);
            setVacancies(res.data || []);
        } catch (err) {
            console.error('Ошибка при загрузке вакансий:', err);
        }
    };

    const fetchAvailableFilters = async () => {
        try {
            const response= await api.get(API.filter.get);
            setAvailableFilters(response.data || []);
        } catch (err) {
            console.error('Ошибка при загрузке фильтров:', err);
        }
    };

    const fetchEducation = async () => {
        try {
            const response = await api.get(API.filter.education);
            setEducationList(response.data.education || []);
        } catch (err) {
            console.error('Ошибка при загрузке образования:', err);
        }
    };

    const applyFilters = () => {
        setAppliedFilters(filters); // <-- обновляем appliedFilters при нажатии
    };

    const resetFilters = () => {
        const reset = {
            city: '',
            min_experience: 0,
            max_experience: 100,
            education: '',
            min_wantedSalary: 0,
            max_wantedSalary: 1000000,
            type: '',
            direction: [],
            sortItem: '',
            order: 'asc',
            AISearch: '',
            Page: "1",
            PageSize: "20"
        };
        setFilters(reset);
        setAppliedFilters(reset); // сразу применяем
    };

    const handleFilterChange = (e) => {
        const { name, value } = e.target;
        setFilters((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleExperienceSliderChange = (event, newValue) => {
        setFilters(prev => ({
            ...prev,
            min_experience: newValue[0],
            max_experience: newValue[1],
        }));
    };

    const handleSalarySliderChange = (event, newValue) => {
        setFilters(prev => ({
            ...prev,
            min_wantedSalary: newValue[0],
            max_wantedSalary: newValue[1],
        }));
    };

    const getEducationName = (id) =>
        educationList.find(edu => edu.id === id)?.name || 'не указано';

    const truncateDescription = (desc) =>
        desc?.length > 50 ? desc.slice(0, 50) + '...' : desc;

    const showSnackbar = (message, severity = 'success') => {
        setSnackbar({
            open: true,
            message,
            severity
        });
    };

    const handleVacancySelect = (resumeId, vacancyId) => {
        setSelectedVacancyId(prev => ({
            ...prev,
            [resumeId]: vacancyId
        }));
    };

    const handleRespond = async (resumeId, vacancyId) => {
        try {
            await api.post(API.feedback.make, {
                resume_id: resumeId,
                vacancy_id: vacancyId,
            });
            showSnackbar('Отклик успешно отправлен!', 'success');
        } catch (err) {
            console.error('Ошибка при отправке отклика:', err.response?.data || err.message);
            showSnackbar('Ошибка при отправке отклика', 'error');
        }
    };

    const handleResumeClick = (resumeId) => {
        const startTime = Date.now();
        window.location.href = `/Company/Resumes/Info/${resumeId}?sid=${sessionId}&st=${startTime}`;
    };

    if (loading) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <CircularProgress />
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка резюме...</Typography>
            </Box>
        );
    }

    return (
        <Container maxWidth="xl" sx={{ py: 6 }}>
            <Typography variant="h4" fontWeight="bold" gutterBottom align="center">
                Все резюме
            </Typography>

            {/* Фильтры */}
            <Grid container spacing={3} sx={{ mb: 4 }}>
                <Grid item xs={12} md={3}>
                    <Paper elevation={3} sx={{ p: 3, borderRadius: 2 }}>
                        <Typography variant="h6" gutterBottom>Фильтры</Typography>
                        <Stack spacing={2}>
                            <TextField
                                label="Поиск"
                                name="AISearch"
                                value={filters.AISearch}
                                onChange={handleFilterChange}
                                fullWidth
                                helperText="Название или описание"
                            />

                            <TextField
                                label="Город"
                                name="city"
                                value={filters.city}
                                onChange={handleFilterChange}
                                fullWidth
                            />
                            <Box sx={{ mt: 2 }}>
                                <Typography gutterBottom>Опыт работы</Typography>
                                <Slider
                                    name="experience"
                                    value={[filters.min_experience, filters.max_experience]}
                                    onChange={handleExperienceSliderChange}
                                    valueLabelDisplay="auto"
                                    min={0}
                                    max={100}
                                    marks={[
                                        { value: 0, label: '0 лет' },
                                        { value: 100, label: '100' },
                                    ]}
                                />
                            </Box>
                            <Box sx={{ mt: 2 }}>
                                <Typography gutterBottom>Желаемая зарплата</Typography>
                                <Slider
                                    name="wantedSalary"
                                    value={[filters.min_wantedSalary, filters.max_wantedSalary]}
                                    onChange={handleSalarySliderChange}
                                    valueLabelDisplay="auto"
                                    min={0}
                                    max={1000000}
                                    step={5000}
                                    marks={[
                                        { value: 0, label: '0 ₽' },
                                        { value: 1000000, label: '1 000 000 ₽' }
                                    ]}
                                    valueLabelFormat={(value) => `${value} ₽`}
                                />
                            </Box>
                            <FormControl fullWidth>
                                <InputLabel id="filter-education">Образование</InputLabel>
                                <Select
                                    labelId="filter-education"
                                    name="education"
                                    value={filters.education}
                                    onChange={handleFilterChange}
                                    label="Образование"
                                >
                                    <MenuItem key="all" value="">Все образования</MenuItem>
                                    {educationList.map(edu => (
                                        <MenuItem key={edu.id} value={edu.id}>
                                            {edu.name}
                                        </MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <FormControl fullWidth>
                                <InputLabel id="filter-type">Тип деятельности</InputLabel>
                                <Select
                                    labelId="filter-type"
                                    name="type"
                                    value={filters.type}
                                    onChange={handleFilterChange}
                                    label="Тип деятельности"
                                >
                                    {[...new Set(availableFilters.map(f => f.type))].map(type => (
                                        <MenuItem key={type} value={type}>{type}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                            <FormControl fullWidth>
                                <InputLabel id="filter-direction">Направления</InputLabel>
                                <Select
                                    labelId="filter-direction"
                                    multiple
                                    name="direction"
                                    value={filters.direction}
                                    onChange={handleFilterChange}
                                    renderValue={(selected) => (
                                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                            {selected.map(value => (
                                                <Chip key={value} label={value} />
                                            ))}
                                        </Box>
                                    )}
                                    label="Направления"
                                >
                                    {availableFilters
                                        .filter(f => !filters.type || f.type === filters.type)
                                        .map(f => (
                                            <MenuItem key={f.id} value={f.direction}>
                                                {f.direction}
                                            </MenuItem>
                                        ))
                                    }
                                </Select>
                            </FormControl>
                            <FormControl fullWidth>
                                <InputLabel id="sort-item">Сортировать по</InputLabel>
                                <Select
                                    labelId="sort-item"
                                    name="sortItem"
                                    value={filters.sortItem}
                                    onChange={handleFilterChange}
                                    label="Сортировать по"
                                >
                                    <MenuItem value="">Без сортировки</MenuItem>
                                    <MenuItem value="city">Городу</MenuItem>
                                    <MenuItem value="experience">Опыту</MenuItem>
                                    <MenuItem value="income_date">Дате добавления</MenuItem>
                                </Select>
                            </FormControl>
                            <FormControl fullWidth>
                                <InputLabel id="order-label">Порядок</InputLabel>
                                <Select
                                    labelId="order-label"
                                    name="order"
                                    value={filters.order}
                                    onChange={handleFilterChange}
                                    label="Порядок"
                                >
                                    <MenuItem value="asc">По возрастанию</MenuItem>
                                    <MenuItem value="desc">По убыванию</MenuItem>
                                </Select>
                            </FormControl>
                            <Button variant="contained" onClick={applyFilters}>Применить</Button>
                            <Button variant="outlined" onClick={resetFilters}>Сбросить</Button>
                        </Stack>
                    </Paper>
                </Grid>

                {/* Резюме */}
                <Grid item xs={12} md={9}>
                    {resumes.length === 0 && (
                        <Paper elevation={3} sx={{ p: 4, bgcolor: '#fff3cd', mb: 3 }}>
                            <Typography align="center" color="text.secondary">
                                Нет доступных резюме.
                            </Typography>
                        </Paper>
                    )}

                    <Stack spacing={3}>
                        {resumes.map(resume => {
                            const selectedVacancy = selectedVacancyId[resume.id];
                            const educationName = getEducationName(resume.education_id);

                            return (
                                <Paper key={resume.id} elevation={3} sx={{ p: 3, borderRadius: 3 }}>
                                    <Box display="flex" justifyContent="space-between" alignItems="start">
                                        <Box sx={{ flex: 1, mr: 2 }}>
                                            <Typography variant="h6" fontWeight="bold">
                                                📄 {resume.post || 'Резюме'}
                                            </Typography>
                                            <Typography variant="body2" color="text.secondary">
                                                Город: {resume.city}
                                            </Typography>
                                            <Divider sx={{ my: 1 }} />
                                            <Typography variant="body1" sx={{ mb: 1 }}>
                                                <strong>Описание:</strong> {truncateDescription(resume.skill)}
                                            </Typography>
                                            <Typography variant="body1" sx={{ mb: 1 }}>
                                                <strong>Опыт:</strong> {resume.experience} лет
                                            </Typography>
                                            <Typography variant="body1" sx={{ mb: 1 }}>
                                                <strong>Желаемая зарплата:</strong> {resume.wantedSalary} ₽
                                            </Typography>
                                            <Typography variant="body1" sx={{ mb: 1 }}>
                                                <strong>Образование:</strong> {educationName}
                                            </Typography>
                                            <Typography variant="body1" sx={{ mb: 1 }}>
                                                <strong>Дата добавления:</strong> {dayjs(resume.income_date).format('DD.MM.YYYY')}
                                            </Typography>
                                            <Box sx={{ mt: 1, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                                {resume.activities?.map(act => (
                                                    <Chip
                                                        key={act.id}
                                                        label={act.direction}
                                                        sx={{ bgcolor: '#e3f2fd', color: '#1976d2' }}
                                                    />
                                                ))}
                                            </Box>
                                        </Box>

                                        <Box sx={{ flex: 1, ml: 2, minWidth: 200 }}>
                                            {userRole === 'Company' && (
                                                <FormControl fullWidth>
                                                    <InputLabel id={`vacancy-select-label-${resume.id}`}>
                                                        Выберите вакансию
                                                    </InputLabel>
                                                    <Select
                                                        labelId={`vacancy-select-label-${resume.id}`}
                                                        value={selectedVacancy || ''}
                                                        onChange={(e) => handleVacancySelect(resume.id, e.target.value)}
                                                        label="Выберите вакансию"
                                                    >
                                                        {vacancies.map(vacancy => {
                                                            const salaryDisplay = vacancy.max_salary
                                                                ? `${vacancy.min_salary} - ${vacancy.max_salary}`
                                                                : vacancy.min_salary;

                                                            return (
                                                                <MenuItem key={vacancy.id} value={vacancy.id}>
                                                                    <Box>
                                                                        <Typography variant="body2" fontWeight="bold">
                                                                            {vacancy.post}
                                                                        </Typography>
                                                                        <Typography variant="caption">
                                                                            Зарплата: {salaryDisplay} ₽
                                                                        </Typography>
                                                                        <Typography variant="caption">
                                                                            Образование: {getEducationName(vacancy.education_id)}
                                                                        </Typography>
                                                                        <Typography variant="caption">
                                                                            Опыт: {vacancy.experience || '—'}
                                                                        </Typography>
                                                                    </Box>
                                                                </MenuItem>
                                                            );
                                                        })}
                                                    </Select>
                                                </FormControl>
                                            )}
                                            <ButtonGroup orientation={"vertical"} fullWidth sx={{ mt: 1, py: 1.2 }}>
                                                {userRole === 'Company' && (
                                                    <Button
                                                        variant="contained"
                                                        color="primary"
                                                        disabled={!selectedVacancy}
                                                        onClick={() => handleRespond(resume.id, selectedVacancy)}
                                                    >
                                                        Откликнуться
                                                    </Button>
                                                )}
                                                <Button
                                                    variant="contained"
                                                    color="secondary"
                                                    onClick={() => handleResumeClick(resume.id)}
                                                >
                                                    Посмотреть подробнее
                                                </Button>
                                            </ButtonGroup>
                                        </Box>
                                    </Box>
                                </Paper>
                            );
                        })}
                    </Stack>
                </Grid>
            </Grid>

            <Snackbar
                open={snackbar.open}
                autoHideDuration={3000}
                onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}
            >
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}