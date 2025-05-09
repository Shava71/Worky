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
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Chip,
    CircularProgress,
} from '@mui/material';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

export default function MyVacancy() {
    const [vacancies, setVacancies] = useState([]);
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [editingVacancy, setEditingVacancy] = useState(null);
    const [modalOpen, setModalOpen] = useState(false);
    const [selectedDirections, setSelectedDirections] = useState([]);
    const [availableFilters, setAvailableFilters] = useState([]);
    const [educationList, setEducationList] = useState([]);
    const navigate = useNavigate();

    // Загрузка данных
    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('jwt');
                if (!token) return;

                const vacanciesResponse = await axios.get('https://localhost:7106/api/v1/Company/MyVacancy', {
                    headers: { Authorization: `Bearer ${token}` },
                });

                const filtersResponse = await axios.get('https://localhost:7106/api/v1/GetInfo/Filter', {
                    headers: { Authorization: `Bearer ${token}` },
                });

                const educationResponse = await axios.get('https://localhost:7106/api/v1/GetInfo/Education', {
                    headers: { Authorization: `Bearer ${token}` },
                });

                setVacancies(vacanciesResponse.data || []);
                setAvailableFilters(filtersResponse.data.filters || []);
                setEducationList(educationResponse.data.education || []);
            } catch (err) {
                console.error('Ошибка при загрузке данных:', err);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить данные',
                    severity: 'error',
                });
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    // Открытие модального окна
    const handleOpenEditModal = (vacancy) => {
        setEditingVacancy(vacancy);
        setSelectedDirections(vacancy.activities?.map(a => a.id) || []);
        setModalOpen(true);
    };

    const handleCloseModal = () => {
        setModalOpen(false);
        setEditingVacancy(null);
        setSelectedDirections([]);
    };

    const handleInputChange = (e) => {
        setEditingVacancy({
            ...editingVacancy,
            [e.target.name]: e.target.value,
        });
    };

    const handleDirectionChange = (e) => {
        setSelectedDirections(e.target.value);
    };

    const handleSaveChanges = async () => {
        try {
            const token = localStorage.getItem('jwt');

            const updatedData = {
                Id: Number(editingVacancy.id),
                Post: editingVacancy.post,
                MinSalary: Number(editingVacancy.min_salary),
                MaxSalary: editingVacancy.max_salary ? Number(editingVacancy.max_salary) : null,
                EducationId: Number(editingVacancy.education_id),
                Experience: editingVacancy.experience?.toString() ?? '',
                Description: editingVacancy.description,
            };

            // 1. Обновляем саму вакансию
            await axios.put('https://localhost:7106/api/v1/Company/UpdateVacancy', updatedData, {
                headers: { Authorization: `Bearer ${token}` },
            });

            // 2. Удаляем старые фильтры
            if (editingVacancy.activities?.length > 0) {
                const oldFilterIds = editingVacancy.activities.map(a => a.filter_id); // ❗ Теперь по `filter_id`
                for (const id of oldFilterIds) {
                    await axios.delete('https://localhost:7106/api/v1/Company/DeleteVacancyFilter', {
                        params: { filterId: id },
                        headers: { Authorization: `Bearer ${token}` },
                    });
                }
            }

            // 3. Добавляем новые фильтры
            if (selectedDirections.length > 0) {
                await axios.post('https://localhost:7106/api/v1/Company/AddVacancyFilter', {
                    id: editingVacancy.id,
                    typeOfActivity_id: selectedDirections,
                }, {
                    headers: { Authorization: `Bearer ${token}` },
                });
            }

            // 4. Обновляем локальный список
            setVacancies(prev =>
                prev.map(v =>
                    v.id === editingVacancy.id
                        ? { ...v, ...updatedData, activities: availableFilters.filter(f => selectedDirections.includes(f.id)) }
                        : v
                )
            );

            setSnackbar({
                open: true,
                message: 'Вакансия успешно обновлена!',
                severity: 'success',
            });

            handleCloseModal();
        } catch (err) {
            console.error('Ошибка сохранения:', err.response?.data || err.message);
            setSnackbar({
                open: true,
                message: 'Ошибка при сохранении вакансии',
                severity: 'error',
            });
        }
    };

    const handleDeleteVacancy = async (id) => {
        try {
            const token = localStorage.getItem('jwt');
            await axios.delete('https://localhost:7106/api/v1/Company/DeleteVacancy', {
                params: { vacancyId: id },
                headers: { Authorization: `Bearer ${token}` },
            });
            setVacancies(vacancies.filter(v => v.id !== id));
            setSnackbar({
                open: true,
                message: 'Вакансия удалена!',
                severity: 'success',
            });
        } catch {
            setSnackbar({
                open: true,
                message: 'Ошибка при удалении вакансии',
                severity: 'error',
            });
        }
    };

    const handleDeleteFilter = async (filterId) => {
        try {
            const token = localStorage.getItem('jwt');

            await axios.delete('https://localhost:7106/api/v1/Company/DeleteVacancyFilter', {
                params: { filterId: filterId },
                headers: { Authorization: `Bearer ${token}` },
            });

            setEditingVacancy(prev => ({
                ...prev,
                activities: prev.activities.filter(act => act.filter_id !== filterId), // ❗ Удаляем по `filter_id`
            }));

            setSnackbar({
                open: true,
                message: 'Фильтр удален из вакансии',
                severity: 'success',
            });
        } catch {
            setSnackbar({
                open: true,
                message: 'Ошибка при удалении фильтра',
                severity: 'error',
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar((prev) => ({ ...prev, open: false }));
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
                Мои вакансии ({vacancies.length})
            </Typography>

            {vacancies.length === 0 && (
                <Paper elevation={3} sx={{ p: 4, bgcolor: '#fff3cd', borderRadius: 3 }}>
                    <Typography align="center" color="text.secondary">
                        У вас пока нет активных вакансий.
                    </Typography>
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={() => navigate('/CreateVacancy')}
                        fullWidth
                        sx={{ mt: 2, py: 1.5 }}
                    >
                        Создать новую вакансию
                    </Button>
                </Paper>
            )}

            <Stack spacing={3}>
                {vacancies.map((vacancy) => {
                    const salaryDisplay = vacancy.max_salary
                        ? `${vacancy.min_salary} - ${vacancy.max_salary}`
                        : vacancy.min_salary;

                    const educationName = educationList.find(edu => edu.id === vacancy.education_id)?.name || 'не требуется';

                    return (
                        <Paper key={vacancy.id} elevation={3} sx={{ p: 3, borderRadius: 3 }}>
                            <Box display="flex" justifyContent="space-between" alignItems="start">
                                {/* Левая часть: должность */}
                                <Box sx={{ flex: 1, mr: 2 }}>
                                    <Typography variant="h6" fontWeight="bold">
                                        📄 {vacancy.post}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        {new Date(vacancy.income_date).toLocaleDateString()}
                                    </Typography>
                                </Box>

                                {/* Центральная часть: описание и поля */}
                                <Box sx={{ flex: 2, ml: 2, mr: 2 }}>
                                    <Typography variant="body1" sx={{ mb: 1, textAlign: 'left', whiteSpace: 'pre-line' }}>
                                        <strong>Описание:</strong><br/>
                                        {vacancy.description || '—'}
                                    </Typography>
                                    <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                        <strong>Зарплата:</strong> {salaryDisplay} ₽
                                    </Typography>
                                    <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                        <strong>Образование:</strong> {educationName}
                                    </Typography>
                                    <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                        <strong>Опыт работы:</strong> {vacancy.experience || '—'}
                                    </Typography>
                                    <Box sx={{ mt: 1, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                        {vacancy.activities?.map((act) => (
                                            <Chip
                                                key={act.filter_id}
                                                label={act.direction}
                                                onDelete={() => handleDeleteFilter(act.filter_id)}
                                                deleteIcon={<span style={{ fontSize: '1rem' }}>&times;</span>}
                                                sx={{
                                                    bgcolor: '#e3f2fd',
                                                    color: '#1976d2',
                                                    '& .MuiChip-deleteIcon': {
                                                        color: '#1976d2',
                                                        fontSize: '1.2rem',
                                                    },
                                                }}
                                            />
                                        ))}
                                    </Box>
                                </Box>

                                {/* Правая часть: действия */}
                                <Box sx={{ flex: 1, ml: 2, textAlign: 'right' }}>
                                    <Button size="small" onClick={() => handleOpenEditModal(vacancy)}>
                                        Редактировать
                                    </Button>
                                    <Button size="small" color="error" onClick={() => handleDeleteVacancy(vacancy.id)}>
                                        Удалить
                                    </Button>
                                </Box>
                            </Box>
                        </Paper>
                    );
                })}
            </Stack>

            {/* Модальное окно редактирования */}
            <Dialog open={modalOpen} onClose={handleCloseModal}>
                <DialogTitle>Редактирование вакансии</DialogTitle>
                <DialogContent>
                    <TextField
                        label="Должность"
                        name="post"
                        value={editingVacancy?.post || ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <TextField
                        label="Минимальная зарплата"
                        name="min_salary"
                        type="number"
                        value={editingVacancy?.min_salary || ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <TextField
                        label="Максимальная зарплата"
                        name="max_salary"
                        type="number"
                        value={editingVacancy?.max_salary ?? ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />

                    {/* Селект с образованием */}
                    <FormControl fullWidth sx={{ mt: 2 }}>
                        <InputLabel id="education-label">Выберите образование</InputLabel>
                        <Select
                            labelId="education-label"
                            label="Выберите образование"
                            name="education_id"
                            value={editingVacancy?.education_id || ''}
                            onChange={handleInputChange}
                        >
                            {educationList.map((edu) => (
                                <MenuItem key={edu.id} value={edu.id}>
                                    {edu.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    <TextField
                        label="Опыт работы"
                        name="experience"
                        value={editingVacancy?.experience || ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <TextField
                        label="Описание"
                        name="description"
                        value={editingVacancy?.description || ''}
                        onChange={handleInputChange}
                        multiline
                        rows={4}
                        fullWidth
                        sx={{ mt: 2 }}
                    />

                    {/* Фильтры */}
                    <FormControl fullWidth sx={{ mt: 2 }}>
                        <InputLabel id="filter-label">Выберите фильтры</InputLabel>
                        <Select
                            labelId="filter-label"
                            multiple
                            value={selectedDirections}
                            onChange={handleDirectionChange}
                            renderValue={(selected) => (
                                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                    {selected.map(id => {
                                        const activity = availableFilters.find(f => f.id === id);
                                        return activity ? (
                                            <Chip
                                                key={activity.id}
                                                label={activity.direction}
                                                onDelete={() => handleDeleteFilter(activity.filter_id)} // ❗ Здесь тоже по `filter_id`
                                                sx={{
                                                    bgcolor: '#e3f2fd',
                                                    color: '#1976d2',
                                                    '& .MuiChip-deleteIcon': {
                                                        color: '#1976d2',
                                                        fontSize: '1.2rem',
                                                    },
                                                }}
                                            />
                                        ) : null;
                                    })}
                                </Box>
                            )}
                        >
                            {availableFilters.map((dir) => (
                                <MenuItem key={dir.id} value={dir.id}>
                                    {dir.direction}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </DialogContent>
                <DialogActions>
                    <Button onClick={handleCloseModal}>Отмена</Button>
                    <Button onClick={handleSaveChanges} variant="contained">
                        Сохранить изменения
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={handleCloseSnackbar}>
                <Alert onClose={handleCloseSnackbar} severity={snackbar.severity}>
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </Container>
    );
}