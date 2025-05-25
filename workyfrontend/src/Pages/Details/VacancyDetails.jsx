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
    Alert
} from '@mui/material';
import axios from 'axios';
import dayjs from 'dayjs';
import { useNavigate, useParams } from 'react-router-dom';

export default function VacancyDetailsPage() {
    const [vacancy, setVacancy] = useState(null);
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [userRole, setUserRole] = useState(null); // Роль пользователя
    const [educationList, setEducationList] = useState([]); // Список образований
    const navigate = useNavigate();
    const { vacancyId } = useParams();

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
                const vacancyResponse = await axios.get(`https://localhost:7106/api/v1/Worker/Vacancies/Info`, {
                    headers: { Authorization: `Bearer ${token}` },
                    params: { vacancyId }
                });

                setVacancy(vacancyResponse.data.vacancy?.[0] || null);

                // Загружаем список образований
                const educationResponse = await axios.get('https://localhost:7106/api/v1/GetInfo/Education', {
                    headers: { Authorization: `Bearer ${token}` },
                });

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

    // Функция для получения названия образования по ID
    const getEducationName = (id) => {
        const education = educationList.find(edu => edu.id === id);
        return education ? education.name : 'не указано';
    };

    // Отправка отклика
    const handleRespond = async () => {
        try {
            const token = localStorage.getItem('jwt');
            await axios.post('https://localhost:7106/api/v1/Worker/MakeFeedback', {
                vacancy_id: vacancy.id
            }, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
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
            {userRole === 'Worker' && (
                <Paper elevation={3} sx={{ p: 3, mb: 4, borderRadius: 3 }}>
                    <Typography variant="h6" gutterBottom fontWeight="bold">
                        📝 Откликнуться
                    </Typography>
                    <Button
                        variant="contained"
                        color="primary"
                        fullWidth
                        onClick={handleRespond}
                        sx={{ py: 1.2 }}
                    >
                        Отправить отклик
                    </Button>
                </Paper>
            )}

            <Button onClick={() => navigate(-1)}>← Назад</Button>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}