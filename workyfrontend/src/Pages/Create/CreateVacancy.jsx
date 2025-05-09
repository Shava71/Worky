import React, { useState, useEffect } from 'react';
import {
    Box,
    Container,
    Typography,
    TextField,
    Button,
    Stack,
    Snackbar,
    Alert,
    Paper,
    Divider,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Chip,
    Autocomplete,
    CircularProgress,
} from '@mui/material';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

export default function CreateVacancy() {
    const [formData, setFormData] = useState({
        post: '',
        min_salary: 1,
        max_salary: null,
        education_id: 1,
        experience: '',
        description: '',
    });
    const [filters, setFilters] = useState([]);
    const [selectedType, setSelectedType] = useState('');
    const [selectedDirections, setSelectedDirections] = useState([]);
    const [filteredDirections, setFilteredDirections] = useState([]);

    const [educationList, setEducationList] = useState([]);
    const [loading, setLoading] = useState(true);
    const [loadingEducation, setLoadingEducation] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [noDeal, setNoDeal] = useState(false); // Нет активного договора
    const [limitExceeded, setLimitExceeded] = useState(false); // Лимит вакансий исчерпан
    const navigate = useNavigate();

    // Загрузка образования
    useEffect(() => {
        const fetchEducation = async () => {
            try {
                const response = await axios.get('https://localhost:7106/api/v1/GetInfo/Education');
                setEducationList(response.data.education || []);
            } catch (error) {
                console.error('Ошибка загрузки образования:', error);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить список образования',
                    severity: 'error',
                });
            } finally {
                setLoadingEducation(false);
            }
        };
        fetchEducation();
    }, []);

    // Загрузка фильтров
    useEffect(() => {
        const fetchFilters = async () => {
            try {
                const response = await axios.get('https://localhost:7106/api/v1/GetInfo/Filter');
                setFilters(response.data.filters || []);
            } catch (err) {
                console.error('Ошибка при загрузке фильтров:', err);
            }
        };

        fetchFilters();
    }, []);

    // При изменении типа фильтра обновляем доступные направления
    useEffect(() => {
        if (!selectedType) {
            setFilteredDirections([]);
            return;
        }

        const directions = filters.filter(f => f.type === selectedType);
        setFilteredDirections(directions);
    }, [selectedType, filters]);

    // Загрузка профиля и проверка лимита
    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('jwt');
                if (!token) return;

                const profileResponse = await axios.get('https://localhost:7106/api/v1/Company/GetProfile', {
                    headers: { Authorization: `Bearer ${token}` },
                });

                const deals = profileResponse.data.deals || [];
                const today = new Date();
                const activeDeal = deals.find(deal =>
                    new Date(deal.date_start) <= new Date(today.setDate(today.getDate() + 1)) &&
                    new Date(deal.date_end) >= new Date(today.setDate(today.getDate() - 1))
                );

                if (!activeDeal) {
                    setNoDeal(true);
                    setLimitExceeded(true);
                    setLoading(false);
                    return;
                }

                const tariffId = parseInt(activeDeal.tariff_id);
                const tariffResponse = await axios.get(`https://localhost:7106/api/v1/Company/Tarrif`,
                    {params: {tariffId: tariffId}});
                const vacancyCount = tariffResponse.data.tarrifs?.[0]?.vacancy_count ?? 0;

                const myVacanciesResponse = await axios.get('https://localhost:7106/api/v1/Company/MyVacancy', {
                    headers: { Authorization: `Bearer ${token}` },
                });
                const myVacancyCount = myVacanciesResponse.data.length;

                if (myVacancyCount >= vacancyCount) {
                    setLimitExceeded(true);
                }

                setLoading(false);
            } catch (err) {
                console.error('Ошибка при получении данных:', err);
                setNoDeal(true);
                setLimitExceeded(true);
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleTypeChange = (e) => {
        setSelectedType(e.target.value);
        setSelectedDirections([]); // Очищаем при смене типа
    };

    const handleDirectionChange = (e) => {
        const value = e.target.value;
        setSelectedDirections(value);
    };

    const handleSubmit = async () => {
        try {
            const token = localStorage.getItem('jwt');

            // Создаем вакансию
            const createResponse = await axios.post(
                'https://localhost:7106/api/v1/Company/CreateVacancy',
                formData,
                {
                    headers: { Authorization: `Bearer ${token}` },
                }
            );

            const vacancyId = createResponse.data.id;

            // Добавляем фильтры
            if (selectedDirections.length > 0) {
                await axios.post('https://localhost:7106/api/v1/Company/AddVacancyFilter', {
                    id: vacancyId,
                    typeOfActivity_id: selectedDirections,
                }, {
                    headers: { Authorization: `Bearer ${token}` },
                });
            }

            setSnackbar({
                open: true,
                message: 'Вакансия успешно создана!',
                severity: 'success',
            });

            setTimeout(() => navigate('/Company/Profile'), 1500);
        } catch  {
            setSnackbar({
                open: true,
                message: 'Ошибка при создании вакансии',
                severity: 'error',
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar((prev) => ({ ...prev, open: false }));
    };

    if (loading || loadingEducation) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <CircularProgress />
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка данных...</Typography>
            </Box>
        );
    }

    if (noDeal || limitExceeded) {
        return (
            <Container maxWidth="sm" sx={{ py: 6 }}>
                <Paper elevation={3} sx={{ p: 4, borderRadius: 3, bgcolor: '#fff' }}>
                    <Typography variant="h5" align="center" gutterBottom fontWeight="bold">
                        ⚠️ Ограничение достигнуто
                    </Typography>
                    <Divider sx={{ mb: 3 }} />
                    <Typography variant="body1" color="text.secondary" paragraph align="center">
                        Вы не можете создать новую вакансию.
                    </Typography>
                    {noDeal && (
                        <Typography variant="body1" color="error" align="center">
                            У вас нет активного тарифа. Подключите тариф, чтобы продолжить.
                        </Typography>
                    )}
                    {limitExceeded && (
                        <Typography variant="body1" color="error" align="center">
                            Достигнут лимит вакансий по вашему тарифу. Повысьте тариф для большего количества вакансий.
                        </Typography>
                    )}
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={() => navigate('/Tariffs')}
                        fullWidth
                        sx={{ mt: 3, py: 1.5 }}
                    >
                        Перейти к выбору тарифов
                    </Button>
                </Paper>
            </Container>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 10, mb: 6 }}>
            <Paper elevation={3} sx={{ p: { xs: 3, sm: 4 }, borderRadius: 4, bgcolor: '#fff' }}>
                <Typography variant="h5" fontWeight="bold" align="center" gutterBottom>
                    📝 Выложить вакансию
                </Typography>
                <Divider sx={{ mb: 3 }} />

                <Stack spacing={3}>
                    {/* Поле должности */}
                    <TextField
                        label="Должность"
                        name="post"
                        value={formData.post}
                        onChange={handleChange}
                        fullWidth
                        required
                    />

                    {/* Минимальная зарплата */}
                    <TextField
                        label="Минимальная зарплата*"
                        name="min_salary"
                        type="number"
                        value={formData.min_salary}
                        onChange={handleChange}
                        fullWidth
                    />

                    {/* Максимальная зарплата */}
                    <TextField
                        label="Максимальная зарплата"
                        name="max_salary"
                        type="number"
                        value={formData.max_salary ?? ''}
                        onChange={handleChange}
                        fullWidth
                    />

                    {/* Образование */}
                    <FormControl fullWidth>
                        <InputLabel id="education-label">Требуемое образование*</InputLabel>
                        <Select
                            labelId="education-label"
                            label="Требуемое образование"
                            name="education_id"
                            value={formData.education_id}
                            onChange={handleChange}
                            disabled={loadingEducation}
                        >
                            {loadingEducation ? (
                                <MenuItem disabled>
                                    <CircularProgress size={20} sx={{ mr: 1 }} />
                                    Загрузка...
                                </MenuItem>
                            ) : (
                                educationList.map((edu) => (
                                    <MenuItem key={edu.id} value={edu.id}>
                                        {edu.name}
                                    </MenuItem>
                                ))
                            )}
                        </Select>
                    </FormControl>

                    {/* Выбор типа фильтра */}
                    <FormControl fullWidth>
                        <InputLabel id="filter-type-label">Выберите направление вакансии</InputLabel>
                        <Select
                            labelId="filter-type-label"
                            label="Выберите тип фильтра"
                            value={selectedType}
                            onChange={handleTypeChange}
                        >
                            {[...new Set(filters.map(f => f.type))].map(type => (
                                <MenuItem key={type} value={type}>
                                    {type}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    {/* Мультиселект направлений */}
                    {filteredDirections.length > 0 && (
                        <FormControl fullWidth>
                            <InputLabel id="filter-direction-label">Направления</InputLabel>
                            <Select
                                multiple
                                label="Направления"
                                labelId="filter-direction-label"
                                value={selectedDirections}
                                onChange={handleDirectionChange}
                                renderValue={(selected) => (
                                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                        {selected.map(id => {
                                            const direction = filteredDirections.find(f => f.id === id);
                                            return direction ? (
                                                <Chip key={id} label={direction.direction} color="primary" size="small" sx={{ bgcolor: '#e3f2fd', color: '#1976d2' }} />
                                            ) : null;
                                        })}
                                    </Box>
                                )}
                            >
                                {filteredDirections.map((dir) => (
                                    <MenuItem key={dir.id} value={dir.id}>
                                        {dir.direction}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    )}

                    {/* Опыт работы */}
                    <TextField
                        label="Опыт работы*"
                        name="experience"
                        value={formData.experience}
                        onChange={handleChange}
                        fullWidth
                    />

                    {/* Описание */}
                    <TextField
                        label="Описание"
                        name="description"
                        value={formData.description}
                        onChange={handleChange}
                        multiline
                        rows={8}
                        fullWidth
                        inputProps={{ maxLength: 1000 }}
                        helperText={`${formData.description.length}/1000`}
                    />

                    {/* Кнопка публикации */}
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleSubmit}
                        fullWidth
                        sx={{ py: 1.5 }}
                    >
                        Опубликовать вакансию
                    </Button>
                </Stack>
            </Paper>

            {/* Snackbar уведомление */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={handleCloseSnackbar}>
                <Alert onClose={handleCloseSnackbar} severity={snackbar.severity}>
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </Container>
    );
}