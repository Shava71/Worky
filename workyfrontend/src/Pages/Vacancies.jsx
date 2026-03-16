import React, { useEffect, useState } from 'react';
import {
    Container,
    Paper,
    Typography,
    Box,
    Stack,
    Button,
    Divider,
    TextField,
    Slider,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Chip,
    Grid,
    Snackbar,
    Alert,
    CircularProgress, ButtonGroup,
} from '@mui/material';
import dayjs from 'dayjs';
import qs from 'qs';
import api from '../api/myaxios.js';
import { API } from '../api/routes';

export default function VacanciesPage() {
    const [vacancies, setVacancies] = useState([]);
    const [filters, setFilters] = useState({
        min_experience: 0,
        max_experience: 30,
        min_wantedSalary: 0,
        max_wantedSalary: 1000000,
        education: '',
        type: '',
        direction: [],
        sortItem: '',
        order: 'asc',
        AISearch: '',
        Page: "1",
        PageSize: "20"
    });
    const [appliedFilters, setAppliedFilters] = useState(filters); // <-- новое состояние для запроса
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [educationList, setEducationList] = useState([]);
    const [availableFilters, setAvailableFilters] = useState([]);
    const userRole = localStorage.getItem('role');

    const [myResumes, setMyResumes] = useState([]);
    const [selectedResume, setSelectedResume] = useState('');
    const [sessionId, setSessionId] = useState(null);

    // Загрузка вакансий только при appliedFilters
    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);
            try {
                const response = await api.get(API.search.vacancies,{
                    params: {
                        min_experience: appliedFilters.min_experience || undefined,
                        max_experience: appliedFilters.max_experience || undefined,
                        min_wantedSalary: appliedFilters.min_wantedSalary || undefined,
                        max_wantedSalary: appliedFilters.max_wantedSalary || undefined,
                        education: appliedFilters.education || undefined,
                        type: appliedFilters.type || undefined,
                        direction: appliedFilters.direction.length > 0 ? appliedFilters.direction : undefined,
                        SortItem: appliedFilters.sortItem || undefined,
                        Order: appliedFilters.order || undefined,
                        AISearch: appliedFilters.AISearch || undefined,
                        Page: appliedFilters.Page || undefined,
                        PageSize: appliedFilters.PageSize || undefined
                    },
                    paramsSerializer: (params) => qs.stringify(params, {arrayFormat: 'repeat'}),
                });
                setVacancies(response.data.items.map(x => x.document) || []);
                setSessionId(response.data.sessionId);
                fetchEducation();
            } catch (err) {
                console.error('Ошибка при загрузке вакансий:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchData();
        fetchAvailableFilters();
    }, [appliedFilters]);

    useEffect(() => {
        const fetchMyResumes = async () => {
            try {
                const response = await api.get(API.worker.myResume);
                setMyResumes(response.data.resumes  || []);
            } catch (err) {
                console.error('Ошибка при загрузке резюме:', err);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить ваши резюме',
                    severity: 'error'
                });
            }
        };

        fetchMyResumes();
    }, []);

    const fetchEducation = async () => {
        try {
            const response = await api.get(API.filter.education);
            setEducationList(response.data.education || []);
        } catch (err) {
            console.error('Ошибка при загрузке образования:', err);
        }
    };

    const handleFilterChange = (e) => {
        const { name, value } = e.target;
        setFilters(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const fetchAvailableFilters = async () => {
        try {
            const response = await api.get(API.filter.get);
            setAvailableFilters(response.data || []);
        } catch (err) {
            console.error('Ошибка при загрузке фильтров:', err);
        }
    };

    const handleExperienceSliderChange = (event, newValue) => {
        setFilters(prev => ({
            ...prev,
            min_experience: newValue[0],
            max_experience: newValue[1]
        }));
    };

    const handleSalarySliderChange = (event, newValue) => {
        setFilters(prev => ({
            ...prev,
            min_wantedSalary: newValue[0],
            max_wantedSalary: newValue[1]
        }));
    };

    // Применить фильтры — обновляем appliedFilters
    const applyFilters = () => {
        setAppliedFilters(filters);
    };

    const resetFilters = () => {
        const reset = {
            min_experience: 0,
            max_experience: 100,
            min_wantedSalary: 0,
            max_wantedSalary: 1000000,
            education: '',
            type: '',
            direction: [],
            sortItem: '',
            order: 'asc',
            AISearch: '',
            Page: "1",
            PageSize: "20"
        };
        setFilters(reset);
        setAppliedFilters(reset); // обновляем сразу appliedFilters
    };

    const handleRespond = async (selectedResume, vacancyId) => {
        if (!selectedResume || !vacancyId) return;

        try {
            await api.post(API.feedback.make, {
                resume_id: selectedResume,
                vacancy_id: vacancyId,
            });
            setSnackbar({
                open: true,
                message: 'Отклик успешно отправлен!',
                severity: 'success'
            });
        } catch (err) {
            console.error('Ошибка при отправке отклика:', err.response?.data || err.message);
            setSnackbar({
                open: true,
                message: 'Ошибка при отправке отклика',
                severity: 'error'
            });
        }
    };

    const handleVacancyClick = (vacancyId) => {
        const startTime = Date.now();
        window.location.href = `/Worker/Vacancies/Info/${vacancyId}?sid=${sessionId}&st=${startTime}`;
    };

    if (loading) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <CircularProgress />
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка вакансий...</Typography>
            </Box>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Typography variant="h4" fontWeight="bold" gutterBottom align="center">
                Вакансии компаний
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
                                helperText="Название компании, описание или должность"
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

                {/* Список вакансий */}
                <Grid item xs={12} md={9}>
                    {vacancies.length === 0 && (
                        <Paper elevation={3} sx={{ p: 4, bgcolor: '#fff3cd', mb: 3 }}>
                            <Typography align="center" color="text.secondary">
                                Нет доступных вакансий.
                            </Typography>
                        </Paper>
                    )}

                    <Stack spacing={3}>
                        {vacancies.map(vacancy => (
                            <Paper key={vacancy.id} elevation={3} sx={{ p: 3, borderRadius: 3 }}>
                                <Box  display="flex" justifyContent="space-between" alignItems="start">
                                    {/* Левая часть: информация о вакансии */}
                                    <Box sx={{ flex: 1, mr: 2 }}>
                                        <Typography variant="h6" fontWeight="bold">
                                            📌 {vacancy.post}
                                        </Typography>
                                        <Typography variant="body2" color="text.secondary">
                                            Компания: {vacancy.company.name}
                                        </Typography>
                                        <Divider sx={{ my: 1 }} />
                                        <Typography variant="body1" sx={{ mb: 1 }}>
                                            <strong>Описание:</strong> {vacancy.description}
                                        </Typography>
                                        {
                                            vacancy.maxSalary ? (
                                                <Typography variant="body1" sx={{ mb: 1 }}>
                                                    <strong>Зарплата:</strong> {vacancy.minSalary} - {vacancy.maxSalary}₽
                                                </Typography>
                                            ) :
                                                (<Typography variant="body1" sx={{ mb: 1 }}>
                                                    <strong>Зарплата:</strong> {vacancy.minSalary}₽
                                                </Typography>)
                                        }
                                        <Typography variant="body1" sx={{ mb: 1 }}>
                                            <strong>Образование:</strong> {vacancy.educationName}
                                        </Typography>
                                        <Typography variant="body1" sx={{ mb: 1 }}>
                                            <strong>Опыт:</strong> {vacancy.experience || '—'} лет
                                        </Typography>
                                        <Typography variant="body1" sx={{ mb: 1 }}>
                                            <strong>Дата добавления:</strong> {dayjs(vacancy.income_date).format('DD.MM.YYYY')}
                                        </Typography>
                                        <Box sx={{ mt: 1, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                            {vacancy.activities?.map(act => (
                                                <Chip
                                                    key={act.id}
                                                    label={act.direction}
                                                    sx={{
                                                        bgcolor: '#e3f2fd',
                                                        color: '#1976d2'
                                                    }}
                                                />
                                            ))}
                                        </Box>
                                    </Box>

                                    {/* Правая часть: действия */}
                                    <Box sx={{ flex: 1, ml: 2, minWidth: 200, textAlign: 'right' }}>

                                        {userRole == 'Worker' && (<FormControl fullWidth>
                                            <InputLabel id="resume-select-label">Выберите резюме</InputLabel>
                                            <Select
                                                labelId="resume-select-label"
                                                value={selectedResume}
                                                onChange={(e) => setSelectedResume(e.target.value)}
                                                label="Выберите резюме"
                                            >
                                                {myResumes.map(resume => (
                                                    <MenuItem key={resume.id} value={resume.id}>
                                                        <Box>
                                                            <Typography variant="body2" fontWeight="bold">
                                                                {resume.post || 'Без названия'}
                                                            </Typography>
                                                            <Typography variant="caption">
                                                                Опыт: {resume.experience ?? '—'} лет
                                                            </Typography>
                                                            <Typography variant="caption">
                                                                Желаемая зарплата: {resume.wantedSalary ?? '—'} ₽
                                                            </Typography>
                                                        </Box>
                                                    </MenuItem>
                                                ))}
                                            </Select>
                                        </FormControl>)
                                        }
                                        <ButtonGroup orientation={"vertical"}  fullWidth sx={{ mt: 1, py: 1.2 }}>
                                            {userRole == 'Worker' && (
                                                <Button
                                                    variant="contained"
                                                    color="primary"
                                                    disabled={!selectedResume}
                                                    onClick={() => handleRespond(selectedResume, vacancy.id)}
                                                >
                                                    Откликнуться
                                                </Button>)
                                            }

                                            <Button
                                                variant="contained"
                                                color="secondary"
                                                onClick={() => handleVacancyClick(vacancy.id)}
                                            >
                                                Посмотреть подробнее
                                            </Button>
                                        </ButtonGroup>
                                    </Box>
                                </Box>
                            </Paper>
                        ))}
                    </Stack>
                </Grid>
            </Grid>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}