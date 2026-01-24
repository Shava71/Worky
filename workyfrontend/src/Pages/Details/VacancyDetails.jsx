import React, { useEffect, useState } from 'react';
import {
    Container,
    Paper,
    Typography,
    Box,
    Stack,
    Button,
    Divider,
    Chip,
    CircularProgress,
    Snackbar,
    Alert, FormControl, InputLabel, Select, MenuItem
} from '@mui/material';
import axios from 'axios';
import dayjs from 'dayjs';
import { useNavigate, useParams } from 'react-router-dom';
import api from '../../api/axios';
import { API } from '../../api/routes';

export default function VacancyDetailsPage() {
    const [vacancy, setVacancy] = useState(null);
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [userRole, setUserRole] = useState(null); // Роль пользователя
    const [educationList, setEducationList] = useState([]); // Список образований
    const navigate = useNavigate();
    const { vacancyId } = useParams();

    const [myResumes, setMyResumes] = useState([]);
    const [selectedResume, setSelectedResume] = useState('');

    // Получение данных о вакансии и образовании
    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('jwt');
                if (!token) {
                    navigate('/login');
                    return;
                }

                // Получаем роль пользователя
                const role = localStorage.getItem('role');
                setUserRole(role);

                // Загружаем данные вакансии
                // const vacancyResponse = await axios.get(`https://localhost:7106/api/v1/Worker/Vacancies/Info`, {
                //     headers: { Authorization: `Bearer ${token}` },
                //     params: { vacancyId }
                // });
                const vacancyResponse = await axios.get(API.company.vacancyInfo,
                    {
                        params: {
                            vacancyId
                        }
                    });

                setVacancy(vacancyResponse.data.vacancy?.[0] || null);

                // Загружаем список образований
                // const educationResponse = await axios.get('https://localhost:7106/api/v1/GetInfo/Education', {
                //     headers: { Authorization: `Bearer ${token}` },
                // });
                const educationResponse = await api.get(API.filter.education);

                setEducationList(educationResponse.data.education || []);
            } catch (err) {
                console.error('Ошибка при загрузке данных:', err);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить данные вакансии или образования',
                    severity: 'error'
                });
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [vacancyId]);

    useEffect(() => {
        const fetchMyResumes = async () => {
            try {
                // const token = localStorage.getItem('jwt');
                // const response = await axios.get('https://localhost:7106/api/v1/Worker/MyResume',  {
                //     headers: { Authorization: `Bearer ${token}` },
                // });
                const response = await axios.get(API.worker.myResume);
                setMyResumes(response.data || []);
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

    // Функция для получения названия образования по ID
    const getEducationName = (id) => {
        const education = educationList.find(edu => edu.id === id);
        return education ? education.name : 'не указано';
    };

    // Отправка отклика
    const handleRespond = async (selectedResume, vacancyId) => {
        if (!selectedResume || !vacancyId) return;

        try {
            // const token = localStorage.getItem('jwt');
            // await axios.post(
            //     'https://localhost:7106/api/v1/Worker/MakeFeedback',
            //     {
            //         resume_id: selectedResume,
            //         vacancy_id: vacancyId
            //     },
            //     {
            //         headers: {
            //             Authorization: `Bearer ${token}`,
            //             'Content-Type': 'application/json'
            //         }
            //     }
            // );
            await api.post(API.feedback.make, {
                resume_id: selectedResume,
                vacancy_id: vacancyId
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
    if (loading) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <CircularProgress />
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка вакансии...</Typography>
            </Box>
        );
    }

    if (!vacancy) {
        return (
            <Container maxWidth="sm" sx={{ py: 6 }}>
                <Paper elevation={3} sx={{ p: 4, borderRadius: 3 }}>
                    <Typography align="center" color="text.secondary">
                        Вакансия не найдена.
                    </Typography>
                    <Button fullWidth onClick={() => navigate(-1)} sx={{ mt: 2 }}>
                        Назад к списку
                    </Button>
                </Paper>
            </Container>
        );
    }

    const salaryDisplay = vacancy.max_salary
        ? `${vacancy.min_salary} — ${vacancy.max_salary}`
        : vacancy.min_salary;

    return (
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Paper elevation={3} sx={{ p: 3, mb: 4, borderRadius: 3 }}>
                <Box display="flex" justifyContent="space-between" gap={3}>
                    {/* Информация о компании */}
                    <Box sx={{
                        flex: 1,
                        minWidth: 200,
                        borderRight: '1px solid #ccc',
                        pr: 2,
                        mr: 2
                    }}>
                        <Typography variant="h6" fontWeight="bold">Компания</Typography>
                        <Divider sx={{ my: 1 }} />
                        <Typography><strong>Название:</strong> {vacancy.company?.name}</Typography>
                        <Typography><strong>Email:</strong> {vacancy.company?.email || '—'}</Typography>
                        <Typography><strong>Телефон:</strong> {vacancy.company?.phoneNumber || '—'}</Typography>
                        <Typography><strong>Сайт:</strong> {vacancy.company?.website || '—'}</Typography>
                        <Typography><strong>Адрес:</strong> {vacancy.company?.latitude}, {vacancy.company?.longitude}</Typography>
                    </Box>

                    {/* Основная информация о вакансии */}
                    <Box sx={{ flex: 2 }}>
                        <Typography variant="h5" gutterBottom fontWeight="bold">
                            {vacancy.post}
                        </Typography>
                        <Typography paragraph sx={{ whiteSpace: 'pre-line' }}>
                            {vacancy.description}
                        </Typography>
                        <Box sx={{ mt: 2 }}>
                            <Typography><strong>Опыт работы:</strong> {vacancy.experience} лет</Typography>
                            <Typography><strong>Зарплата:</strong> {salaryDisplay} ₽</Typography>
                            <Typography><strong>Образование:</strong> {getEducationName(vacancy.education_id)}</Typography>
                            <Typography><strong>Дата добавления:</strong> {dayjs(vacancy.income_date).format('DD.MM.YYYY')}</Typography>
                        </Box>
                        <Box sx={{ mt: 2, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                            {vacancy.activities?.map((act) => (
                                <Chip key={act.id} label={act.direction} sx={{
                                    bgcolor: '#e3f2fd',
                                    color: '#1976d2'
                                }} />
                            ))}
                        </Box>
                    </Box>
                </Box>
            </Paper>

            {/* Кнопка "Откликнуться" только для Worker */}
            {userRole === 'Worker' &&
                (

                    <Paper elevation={3} sx={{ p: 3, mb: 4, borderRadius: 3 }}>
                        <Typography variant="h6" gutterBottom fontWeight="bold">
                            📝 Откликнуться
                        </Typography>
                        <FormControl fullWidth>
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
                        </FormControl>
                        <Button
                            variant="contained"
                            color="primary"
                            fullWidth
                            onClick={() => handleRespond(selectedResume, vacancy.id)}
                            sx={{ py: 1.2 }}
                            disabled={!selectedResume}
                        >
                            Отправить отклик
                        </Button>
                    </Paper>
                )
            }

            <Button onClick={() => navigate(-1)}>← Назад</Button>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}