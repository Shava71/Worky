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
    Snackbar,
    Alert,
    Grid,
    CircularProgress,
} from '@mui/material';
import axios from 'axios';
import dayjs from 'dayjs';
import { useNavigate } from 'react-router-dom';

export default function WorkerFeedbackPage() {
    const [feedbacks, setFeedbacks] = useState([]);
    const [vacancies, setVacancies] = useState({});
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const navigate = useNavigate();

    // Загрузка откликов
    const fetchFeedbacks = async () => {
        try {
            const token = localStorage.getItem('jwt');
            const res = await axios.get('https://localhost:7106/api/v1/Worker/GetFeedback',  {
                headers: { Authorization: `Bearer ${token}` },
            });

            setFeedbacks(res.data.feedbacks || []);
        } catch (err) {
            console.error('Ошибка при загрузке откликов:', err);
            setSnackbar({
                open: true,
                message: 'Не удалось загрузить список откликов',
                severity: 'error',
            });
        }
    };

    // Загрузка информации по конкретной вакансии
    const fetchVacancyInfo = async (vacancyId) => {
        try {
            const token = localStorage.getItem('jwt');
            const res = await axios.get(`https://localhost:7106/api/v1/Worker/Vacancies/Info`,  {
                headers: { Authorization: `Bearer ${token}` },
                params: { vacancyId: vacancyId },
            });

            setVacancies((prev) => ({
                ...prev,
                [vacancyId]: res.data.vacancy?.[0] || null,
            }));
        } catch (err) {
            console.error('Ошибка при загрузке вакансии:', err);
            setVacancies((prev) => ({
                ...prev,
                [vacancyId]: null,
            }));
        }
    };

    // Инициальная загрузка данных
    useEffect(() => {
        const fetchData = async () => {
            await Promise.all([fetchFeedbacks()]);
            setLoading(false);
        };
        fetchData();
    }, []);

    // Получаем данные по вакансии при первом открытии
    useEffect(() => {
        feedbacks.forEach(fb => {
            if (!vacancies[fb.vacancy_id]) {
                fetchVacancyInfo(fb.vacancy_id);
            }
        });
    }, [feedbacks]);

    // Форматирование даты
    const formatDate = (date) => dayjs(date).format('DD.MM.YYYY');

    // Перейти к вакансии
    const handleOpenVacancyDetails = (vacancyId) => {
        navigate(`/Worker/Vacancies/Info/${vacancyId}`);
    };

    const handleDeleteVacancy = async (id) => {
        try {
            const token = localStorage.getItem('jwt');
            await axios.delete(`https://localhost:7106/api/v1/Worker/DeleteFeedback`,  {
                params: { id: id },
                headers: { Authorization: `Bearer ${token}` },
            });

            // Обновляем локальное состояние
            setFeedbacks((prev) => prev.filter((v) => v.id !== id));

            // Показываем уведомление
            setSnackbar({
                open: true,
                message: 'Отклик успешно удален!',
                severity: 'success',
            });
        } catch (err) {
            console.error('Ошибка при удалении отклика:', err);
            setSnackbar({
                open: true,
                message: 'Не удалось удалить отклик',
                severity: 'error',
            });
        }
    };

    if (loading) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <CircularProgress />
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка откликов...</Typography>
            </Box>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Typography variant="h4" fontWeight="bold" gutterBottom align="center">
                Мои отклики
            </Typography>

            {feedbacks.length === 0 ? (
                <Paper elevation={3} sx={{ p: 4, bgcolor: '#fff3cd', mb: 3, borderRadius: 3 }}>
                    <Typography align="center" color="text.secondary">
                        Вы ещё не отправили ни одного отклика.
                    </Typography>
                    <Button
                        fullWidth
                        variant="contained"
                        color="primary"
                        onClick={() => navigate('/Worker/Vacancies')}
                        sx={{ mt: 2, py: 1.5 }}
                    >
                        Посмотреть доступные вакансии
                    </Button>
                </Paper>
            ) : (
                <Stack spacing={3}>
                    {feedbacks.map(feedback => {
                        const vacancy = vacancies[feedback.vacancy_id];
                        const statusText =
                            feedback.status === 0
                                ? 'В процессе'
                                : feedback.status === 1
                                    ? 'Принято'
                                    : feedback.status === 2
                                        ? 'Отклонено'
                                        : 'Неизвестный статус';
                        const statusColor =
                            feedback.status === 0
                                ? 'warning'
                                : feedback.status === 1
                                    ? 'success'
                                    : feedback.status === 2
                                        ? 'error'
                                        : 'default';

                        return (
                            <Paper key={feedback.id} elevation={3} sx={{ p: 3, borderRadius: 3 }}>
                                <Grid container spacing={3}>
                                    {/* Информация о вакансии */}
                                    <Grid item xs={12} md={5}>
                                        <Typography variant="h6" fontWeight="bold">📌 Вакансия</Typography>
                                        <Divider sx={{ my: 1 }} />

                                        {vacancy ? (
                                            <>
                                                <Typography><strong>Должность:</strong> {vacancy.post}</Typography>
                                                <Typography><strong>Компания:</strong> {vacancy.company.name}</Typography>
                                                <Typography><strong>Описание:</strong> {vacancy.description}</Typography>
                                                <Typography><strong>Зарплата:</strong> {vacancy.min_salary} — {vacancy.max_salary ?? 'не указано'} ₽</Typography>
                                                <Typography><strong>Опыт:</strong> {vacancy.experience} лет</Typography>
                                                <Typography><strong>Образование:</strong> ID {vacancy.education_id}</Typography>
                                            </>
                                        ) : (
                                            <Box>
                                                <Typography color="text.secondary">Загрузка данных о вакансии...</Typography>
                                                <Button size="small" onClick={() => fetchVacancyInfo(feedback.vacancy_id)}>
                                                    Загрузить
                                                </Button>
                                            </Box>
                                        )}
                                    </Grid>

                                    {/* Информация о резюме */}
                                    <Grid item xs={12} md={5}>
                                        <Typography variant="h6" fontWeight="bold">📄 Резюме</Typography>
                                        <Divider sx={{ my: 1 }} />

                                        <Typography><strong>Город:</strong> {feedback.city}</Typography>
                                        <Typography><strong>Опыт:</strong> {feedback.experience} лет</Typography>
                                        <Typography><strong>Желаемая зарплата:</strong> {feedback.wantedSalary} ₽</Typography>
                                        <Typography><strong>Дата подачи:</strong> {formatDate(feedback.income_date)}</Typography>

                                        <Box sx={{ mt: 2, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                            {feedback.activities?.map(act => (
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
                                    </Grid>

                                    {/* Статус и действия */}
                                    <Grid item xs={12} md={2}>
                                        <Typography variant="h6" fontWeight="bold">Статус</Typography>
                                        <Divider sx={{ my: 1 }} />
                                        <Chip
                                            label={statusText}
                                            color={statusColor}
                                            sx={{ mb: 2 }}
                                        />
                                        <Button
                                            variant="contained"
                                            color="info"
                                            fullWidth
                                            onClick={() => handleOpenVacancyDetails(feedback.vacancy_id)}
                                            sx={{ py: 1.2 }}
                                        >
                                            Подробнее
                                        </Button>
                                        { feedback.status === 0 && (
                                        <Button
                                            size="small"
                                            color="error"
                                            onClick={() => handleDeleteVacancy(feedback.id)}
                                        >
                                            Отменить отклик
                                        </Button>)
                                        }
                                    </Grid>
                                </Grid>
                            </Paper>
                        );
                    })}
                </Stack>
            )}

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}