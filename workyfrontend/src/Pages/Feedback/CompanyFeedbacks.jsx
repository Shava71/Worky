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
    CircularProgress,
    Card,
    CardContent,
} from '@mui/material';
// import axios from 'axios';
import dayjs from 'dayjs';
import api from '../../api/myaxios.js';
import { API } from '../../api/routes';

export default function CompanyFeedbackPage() {
    const [feedbacks, setFeedbacks] = useState([]);
    const [vacancies, setVacancies] = useState([]);
    const [resumes, setResumes] = useState({});
    const [selectedVacancyId, setSelectedVacancyId] = useState('');
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

    // Загрузка откликов
    const fetchFeedbacks = async () => {
        try {
            const res = await api.get(API.feedback.get,{
                params:{
                    vacancyId: selectedVacancyId || undefined
                }
            });
            setFeedbacks(res.data || []);
        } catch (err) {
            console.error('Ошибка при загрузке откликов:', err);
        }
    };

    // Загрузка вакансий компании
    const fetchVacancies = async () => {
        try {
            const res = await api.get(API.company.myVacancy);
            setVacancies(res.data.vacancies || []);
        } catch (err) {
            console.error('Ошибка при загрузке вакансий:', err);
        }
    };

    const fetchAllResumes = async () => {
        try {
            const resumeIds = [...new Set(feedbacks.map(fb => fb.resumeId))]; // уникальные ID
            for (const id of resumeIds) {
                if (!resumes[id]) {
                    const res = await api.get(API.worker.resumesInfo, {
                        params: {
                            resumeId: id,
                        }
                    })
                    setResumes(prev => ({
                        ...prev,
                        [id]: res.data.resume || null
                    }));
                }
            }
        } catch (err) {
            console.error('Ошибка при массовой загрузке резюме:', err);
        }
    };

    // Изменение статуса отклика
    const handleChangeStatus = async (feedbackId, newStatus) => {
        try {
            // const token = localStorage.getItem('jwt');
            // await axios.post('https://localhost:7106/api/v1/Company/ChangeFeedbackStatus', {
            //     feedback_id: feedbackId,
            //     status: newStatus
            // }, {
            //     headers: {
            //         Authorization: `Bearer ${token}`,
            //         'Content-Type': 'application/json'
            //     }
            // });
            if(newStatus === 'Accepted') {
                await api.post(API.feedback.accept, {
                    feedbackId: feedbackId,
                });
            }
            else if(newStatus === 'Cancelled') {
                await api.post(API.feedback.cancel, {
                    feedbackId: feedbackId,
                });
            }


            const newStatusPageInfo = newStatus === "Accepted" ? 1 : newStatus === "Cancelled" ? 2 : 0;

            setFeedbacks(prev =>
                prev.map(fb =>
                    fb.id === feedbackId ? { ...fb, status: newStatusPageInfo } : fb
                )
            );

            setSnackbar({
                open: true,
                message: 'Статус успешно изменён!',
                severity: 'success'
            });
        } catch (err) {
            console.error('Ошибка при изменении статуса:', err);
            setSnackbar({
                open: true,
                message: 'Не удалось изменить статус',
                severity: 'error'
            });
        }
    };

    // Выбор вакансии
    const handleVacancyChange = (e) => {
        setSelectedVacancyId(e.target.value || '');
    };

    // Перейти к полному резюме
    const navigateToResumeDetails = (resumeId) => {
        window.location.href = `/Company/Resumes/Info/${resumeId}`;
    };

    // Загрузка начальных данных
    useEffect(() => {
        const fetchData = async () => {
            await Promise.all([fetchFeedbacks(), fetchVacancies()]);
            setLoading(false);
        };
        fetchData();
    }, [selectedVacancyId]);

    useEffect(() => {
        const fetchData = async () => {
            await Promise.all([fetchFeedbacks(), fetchVacancies()]);
            setLoading(false);
        };
        fetchData();
    }, [selectedVacancyId]);

    useEffect(() => {
        if (feedbacks.length > 0) {
            fetchAllResumes();
        }
    }, [feedbacks]);

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

            {/* Выбор вакансии */}
            <Paper elevation={3} sx={{ p: 3, mb: 4, borderRadius: 2 }}>
                <FormControl fullWidth>
                    <InputLabel id="select-vacancy-label">Выберите вакансию</InputLabel>
                    <Select
                        labelId="select-vacancy-label"
                        value={selectedVacancyId}
                        onChange={handleVacancyChange}
                        label="Выберите вакансию"
                    >
                        {vacancies.map(vacancy => (
                            <MenuItem key={vacancy.id} value={vacancy.id}>
                                <Box>
                                    <Typography variant="body2" fontWeight="bold">
                                        {vacancy.post}
                                    </Typography>
                                    <Typography variant="caption">
                                        Зарплата: {vacancy.min_salary} — {vacancy.max_salary ?? 'не указано'}
                                    </Typography>
                                </Box>
                            </MenuItem>
                        ))}
                    </Select>
                </FormControl>
            </Paper>

            {/* Список откликов */}
            <Stack spacing={3}>
                {feedbacks.length === 0 && (
                    <Paper elevation={3} sx={{ p: 4, bgcolor: '#fff3cd' }}>
                        <Typography align="center" color="text.secondary">
                            Нет откликов по выбранной вакансии.
                        </Typography>
                    </Paper>
                )}

                {feedbacks.map(feedback => {
                    const resume = resumes[feedback.resumeId];
                    const vacancy = vacancies.find(v => v.id === feedback.vacancyId);

                    return (
                        <Paper key={feedback.id} elevation={3} sx={{ p: 3, borderRadius: 3 }}>
                            <Grid container spacing={3}>
                                {/* Вакансия */}
                                <Grid item xs={12} md={5}>
                                    <Typography variant="h6" fontWeight="bold" gutterBottom>
                                        📌 {vacancy?.post || '—'}
                                    </Typography>
                                    <Divider sx={{ mb: 2 }} />
                                    <Typography><strong>Описание:</strong> {vacancy?.description}</Typography>
                                    <Typography><strong>Зарплата:</strong> {vacancy?.min_salary} - {vacancy?.max_salary ?? 'не указано'}</Typography>
                                    <Typography><strong>Опыт:</strong> {vacancy?.experience || '—'}</Typography>
                                    <Typography><strong>Образование:</strong> {vacancy?.education_name || '—'}</Typography>
                                </Grid>

                                {/* Резюме */}
                                <Grid item xs={12} md={5}>
                                    <Typography variant="h6" fontWeight="bold" gutterBottom>
                                        👤 Кандидат
                                    </Typography>
                                    <Divider sx={{ mb: 2 }} />

                                    {resume ? (
                                        <>
                                            <Typography><strong>Имя:</strong> {resume.worker.second_name} {resume.worker.first_name}</Typography>
                                            <Typography><strong>Город:</strong> {resume.city}</Typography>
                                            <Typography><strong>Опыт:</strong> {resume.experience} лет</Typography>
                                            <Typography><strong>Желаемая зарплата:</strong> {resume.wantedSalary} ₽</Typography>
                                            <Typography><strong>Дата добавления:</strong> {dayjs(resume.income_date).format('DD.MM.YYYY')}</Typography>
                                            <Box sx={{ mt: 1, display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                                {resume.activities?.map(act => (
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
                                            <Button
                                                onClick={() => navigateToResumeDetails(feedback.resumeId)}
                                                variant="outlined"
                                                color="primary"
                                                sx={{ mt: 2, width: 'fit-content' }}
                                            >
                                                Посмотреть подробнее
                                            </Button>
                                        </>
                                    ) : (
                                        <Button
                                            // onClick={() => fetchResumeInfo(feedback.resume_id)}
                                            variant="contained"
                                            color="info"
                                            sx={{ mt: 2, width: 'fit-content' }}
                                        >
                                            Загрузить резюме
                                        </Button>
                                    )}
                                </Grid>

                                {/* Статус и действия */}
                                <Grid item xs={12} md={2}>
                                    <Typography variant="h6" fontWeight="bold" gutterBottom>
                                        Статус
                                    </Typography>
                                    <Divider sx={{ mb: 2 }} />
                                    <Typography gutterBottom>
                                        {feedback.status === 0 ? 'В процессе' :
                                            feedback.status === 1 ? 'Принято' :
                                                feedback.status === 2 ? 'Отклонено' : 'Неизвестный статус'}
                                    </Typography>
                                    <Button
                                        variant="contained"
                                        color="primary"
                                        fullWidth
                                        disabled={feedback.status !== 0}
                                        onClick={() => handleChangeStatus(feedback.id, "Accepted")}
                                        sx={{ mt: 1 }}
                                    >
                                        Принять
                                    </Button>
                                    <Button
                                        variant="outlined"
                                        color="error"
                                        fullWidth
                                        disabled={feedback.status !== 0}
                                        onClick={() => handleChangeStatus(feedback.id, "Cancelled")}
                                        sx={{ mt: 1 }}
                                    >
                                        Отклонить
                                    </Button>
                                </Grid>
                            </Grid>
                        </Paper>
                    );
                })}
            </Stack>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}