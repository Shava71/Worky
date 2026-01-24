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
// import axios from 'axios';
// import dayjs from 'dayjs';
import { useNavigate } from 'react-router-dom';
import api from '../api/axios';
import { API } from '../api/routes';


export default function MyResume() {
    const [resumes, setResumes] = useState([]);
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [editingResume, setEditingResume] = useState(null);
    const [modalOpen, setModalOpen] = useState(false);
    const [selectedDirections, setSelectedDirections] = useState([]);
    const [availableFilters, setAvailableFilters] = useState([]);
    const [educationList, setEducationList] = useState([]);
    const navigate = useNavigate();

    // Загрузка резюме и фильтров
    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('jwt');
                if (!token) return;

                // const resumesResponse = await axios.get('https://localhost:7106/api/v1/Worker/MyResume', {
                //     headers: { Authorization: `Bearer ${token}` },
                // });
                //
                // const filtersResponse = await axios.get('https://localhost:7106/api/v1/GetInfo/Filter', {
                //     headers: { Authorization: `Bearer ${token}` },
                // });
                //
                // const educationResponse = await axios.get('https://localhost:7106/api/v1/GetInfo/Education', {
                //     headers: { Authorization: `Bearer ${token}` },
                // });
                const resumesResponse = await api.get(API.worker.myResume);

                const filtersResponse = await api.get(API.filter.get);

                const educationResponse = await api.get(API.filter.education);

                setResumes(resumesResponse.data || []);
                setAvailableFilters(filtersResponse.data || []);
                setEducationList(educationResponse.data.education || []);
            } catch (err) {
                console.error('Ошибка при загрузке данных:', err);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить список резюме',
                    severity: 'error',
                });
            } finally {
                setLoading(false);
            }
        };
        fetchData();
    }, []);

    // Открытие модального окна с данными
    const handleOpenEditModal = (resume) => {
        setEditingResume(resume);
        setSelectedDirections(resume.activities?.map(a => a.id) || []);
        setModalOpen(true);
    };

    const handleCloseModal = () => {
        setModalOpen(false);
        setEditingResume(null);
        setSelectedDirections([]);
    };

    // Изменение полей формы
    const handleInputChange = (e) => {
        setEditingResume({
            ...editingResume,
            [e.target.name]: e.target.value,
        });
    };

    // Выбор направлений
    const handleDirectionChange = (e) => {
        setSelectedDirections(e.target.value);
    };

    // Сохранение изменений в резюме
    const handleSaveChanges = async () => {
        try {
            if (!editingResume) return;

            // Обновляем основные данные резюме
            const updatedData = {
                id: editingResume.id,
                post: editingResume.post,
                skill: editingResume.skill,
                city: editingResume.city,
                experience: Number(editingResume.experience),
                education_id: Number(editingResume.education_id),
                wantedSalary: Number(editingResume.wantedSalary),
            };

            await api.put(API.worker.updateResume, updatedData);

            //Старые и новые направления
            const oldActivities = editingResume.activities || [];

            const oldActivityIds = oldActivities.map(a => a.id); // typeOfActivity_id
            const newActivityIds = selectedDirections;           // typeOfActivity_id

            // Diff
            const filtersToAdd = newActivityIds.filter(
                id => !oldActivityIds.includes(id)
            );

            const filtersToRemove = oldActivityIds.filter(
                id => !newActivityIds.includes(id)
            );

            // Удаляем ТОЛЬКО убранные фильтры
            for (const activity of oldActivities) {
                if (filtersToRemove.includes(activity.id)) {
                    await api.delete(API.worker.deleteResumeFilter, {
                        params: { filterId: activity.filter_id },
                    });
                }
            }

            // Добавляем ТОЛЬКО новые фильтры
            if (filtersToAdd.length > 0) {
                await api.post(API.worker.addResumeFilter, {
                    id: editingResume.id,
                    typeOfActivity_id: filtersToAdd,
                });
            }

            // Обновляем локальный стейт
            setResumes(prev =>
                prev.map(r =>
                    r.id === editingResume.id
                        ? {
                            ...r,
                            ...updatedData,
                            activities: availableFilters.filter(f =>
                                newActivityIds.includes(f.id)
                            ),
                        }
                        : r
                )
            );

            setSnackbar({
                open: true,
                message: 'Резюме успешно обновлено!',
                severity: 'success',
            });

            handleCloseModal();
        } catch (err) {
            console.error('Ошибка сохранения резюме:', err.response?.data || err);

            setSnackbar({
                open: true,
                message: 'Ошибка при сохранении резюме',
                severity: 'error',
            });
        }
    };

    // Удаление резюме
    const handleDeleteResume = async (id) => {
        try {
            // const token = localStorage.getItem('jwt');
            // await axios.delete('https://localhost:7106/api/v1/Worker/DeleteResume', {
            //     params: { resumeId: id },
            //     headers: { Authorization: `Bearer ${token}` },
            // });
            await api.delete(API.worker.deleteResume, {
                params: { resumeId: id },
            });
            setResumes(resumes.filter(r => r.id !== id));
            setSnackbar({
                open: true,
                message: 'Резюме удалено!',
                severity: 'success',
            });
        } catch {
            setSnackbar({
                open: true,
                message: 'Ошибка при удалении резюме',
                severity: 'error',
            });
        }
    };

    // Удаление фильтра из резюме
    const handleDeleteFilter = async (filterId) => {
        try {
            // const token = localStorage.getItem('jwt');
            // await axios.delete('https://localhost:7106/api/v1/Worker/DeleteResumeFilter', {
            //     params: { filterId: filterId },
            //     headers: { Authorization: `Bearer ${token}` },
            // });
            await api.delete(API.worker.deleteResumeFilter, {
                params: { filterId },
            });
            setEditingResume(prev => ({
                ...prev,
                activities: prev.activities.filter(act => act.id !== filterId),
            }));
            setSnackbar({
                open: true,
                message: 'Фильтр удален из резюме',
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

    const getEducationName = (id) =>
        educationList.find(edu => edu.id === id)?.name || 'не указано';

    const handleCloseSnackbar = () => {
        setSnackbar((prev) => ({ ...prev, open: false }));
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
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Typography variant="h4" fontWeight="bold" gutterBottom align="center">
                Мои резюме ({resumes.length})
            </Typography>

            {resumes.length === 0 && (
                <Paper elevation={3} sx={{ p: 4, bgcolor: '#fff3cd', borderRadius: 3, mb: 4 }}>
                    <Typography align="center" color="text.secondary">
                        У вас пока нет активных резюме.
                    </Typography>
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={() => navigate('/CreateResume')}
                        fullWidth
                        sx={{ mt: 2, py: 1.5 }}
                    >
                        Создать новое резюме
                    </Button>
                </Paper>
            )}

            <Stack spacing={3}>
                {resumes.map((resume) => (
                    <Paper key={resume.id} elevation={3} sx={{ p: 3, borderRadius: 3 }}>
                        <Box display="flex" justifyContent="space-between" alignItems="start">
                            {/* Левая часть: должность */}
                            <Box sx={{ flex: 1, mr: 2 }}>
                                <Typography variant="h6" fontWeight="bold">
                                    📄 {resume.post}
                                </Typography>
                                <Typography variant="body2" color="text.secondary">
                                    {new Date(resume.income_date).toLocaleDateString()}
                                </Typography>
                            </Box>

                            {/* Центральная часть: информация по резюме */}
                            <Box sx={{ flex: 2, ml: 2, mr: 2 }}>
                                <Typography variant="body1" sx={{ mb: 1, textAlign: 'left', whiteSpace: 'pre-line' }}>
                                    <strong>Описание:</strong><br/>
                                    {resume.skill || '—'}
                                </Typography>
                                <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                    <strong>Город:</strong> {resume.city || '—'}
                                </Typography>
                                <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                    <strong>Опыт:</strong> {resume.experience || '—'} лет
                                </Typography>
                                <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                    <strong>Желаемая зарплата:</strong> {resume.wantedSalary || '—'} ₽
                                </Typography>
                                <Typography variant="body1" sx={{ mb: 1, textAlign: 'left' }}>
                                    <strong>Образование:</strong> {getEducationName(resume.education_id)}
                                </Typography>
                                <Box sx={{ mt: 1, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                    {resume.activities?.map((act) => (
                                        <Chip
                                            key={act.id}
                                            label={act.direction}
                                            sx={{
                                                bgcolor: '#e3f2fd',
                                                color: '#1976d2',
                                            }}
                                        />
                                    ))}
                                </Box>
                            </Box>

                            {/* Правая часть: действия */}
                            <Box sx={{ flex: 1, ml: 2, textAlign: 'right' }}>
                                <Button size="small" onClick={() => handleOpenEditModal(resume)}>
                                    Редактировать
                                </Button>
                                <Button size="small" color="error" onClick={() => handleDeleteResume(resume.id)}>
                                    Удалить
                                </Button>
                            </Box>
                        </Box>
                    </Paper>
                ))}
            </Stack>

            {/* Модальное окно редактирования */}
            <Dialog open={modalOpen} onClose={handleCloseModal}>
                <DialogTitle>Редактирование резюме</DialogTitle>
                <DialogContent>
                    <TextField
                        label="Должность"
                        name="post"
                        value={editingResume?.post || ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <TextField
                        label="Минимальный опыт"
                        name="experience"
                        type="number"
                        value={editingResume?.experience || ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <TextField
                        label="Желаемая зарплата"
                        name="wantedSalary"
                        type="number"
                        value={editingResume?.wantedSalary ?? ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <FormControl fullWidth sx={{ mt: 2 }}>
                        <InputLabel id="education-label">Выберите образование</InputLabel>
                        <Select
                            labelId="education-label"
                            name="education_id"
                            value={editingResume?.education_id || ''}
                            onChange={handleInputChange}
                            label="Выберите образование"
                        >
                            {educationList.map((edu) => (
                                <MenuItem key={edu.id} value={edu.id}>
                                    {edu.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <TextField
                        label="Город"
                        name="city"
                        value={editingResume?.city || ''}
                        onChange={handleInputChange}
                        fullWidth
                        sx={{ mt: 2 }}
                    />
                    <TextField
                        label="Описание"
                        name="skill"
                        value={editingResume?.skill || ''}
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
                                                onDelete={() => handleDeleteFilter(activity.filter_id)}
                                                deleteIcon={<span>&times;</span>}
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