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
    CircularProgress,
    TextField,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Avatar,
    Grid,
} from '@mui/material';
import axios from 'axios';
import dayjs from 'dayjs';
import { useNavigate, useParams } from 'react-router-dom';

export default function ResumeDetailsPage() {
    const [resume, setResume] = useState(null);
    const [vacancies, setVacancies] = useState([]);
    const [educationList, setEducationList] = useState([]);
    const [selectedVacancyId, setSelectedVacancyId] = useState('');
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [loading, setLoading] = useState(true);
    const navigate = useNavigate();
    const { resumeId } = useParams();

    // Загрузка данных о резюме
    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('jwt');
                const [resumeResponse, vacanciesResponse, educationResponse] = await Promise.all([
                    axios.get(`https://localhost:7106/api/v1/Company/Resumes/Info`, {
                        headers: { Authorization: `Bearer ${token}` },
                        params: {
                            resumeId: resumeId,
                        }
                    }),
                    axios.get('https://localhost:7106/api/v1/Company/MyVacancy', {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                    axios.get('https://localhost:7106/api/v1/GetInfo/Education', {
                        headers: { Authorization: `Bearer ${token}` },
                    }),
                ]);

                setResume(resumeResponse.data.resume?.[0] || null);
                setVacancies(vacanciesResponse.data || []);
                setEducationList(educationResponse.data.education || []);
            } catch (err) {
                console.error('Ошибка при загрузке данных:', err);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить данные резюме',
                    severity: 'error'
                });
            } finally {
                setLoading(false);
            }
        };

        fetchData();
    }, [resumeId]);

    const getEducationName = (id) =>
        educationList.find(edu => edu.id === id)?.name || 'не указано';

    const handleRespond = async () => {
        if (!selectedVacancyId) return;
        try {
            const token = localStorage.getItem('jwt');
            await axios.post('https://localhost:7106/api/v1/Company/MakeFeedback', {
                resume_id: resume.id,
                vacancy_id: selectedVacancyId
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
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка резюме...</Typography>
            </Box>
        );
    }

    if (!resume) {
        return (
            <Container maxWidth="sm" sx={{ py: 6 }}>
                <Paper elevation={3} sx={{ p: 4, borderRadius: 3 }}>
                    <Typography align="center" color="text.secondary">
                        Резюме не найдено.
                    </Typography>
                    <Button fullWidth onClick={() => navigate('/Company/Resumes')} sx={{ mt: 2 }}>
                        Назад к списку
                    </Button>
                </Paper>
            </Container>
        );
    }

    const worker = resume.worker;
    const workerName = `${worker.second_name} ${worker.first_name} ${worker.surname}`;

    return (
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Typography variant="h4" fontWeight="bold" gutterBottom align="center">
                Резюме: {resume.post || 'Без названия'}
            </Typography>

            <Grid container spacing={3}>
                {/* Карточка работника */}
                <Grid item xs={12} md={4}>
                    <Paper elevation={3} sx={{
                        p: 3,
                        borderRadius: 3,
                        bgcolor: '#f9f9f9',
                        mb: 3
                    }}>
                        <Stack spacing={2} alignItems="center">
                            <Typography variant="h6" fontWeight="bold">Кандидат</Typography>
                            <Divider sx={{ width: '100%' }} />

                            {/* Фото профиля */}
                            <Box>
                                {worker.image ? (
                                    <Avatar
                                        src={`data:image/jpeg;base64,${btoa(String.fromCharCode.apply(null, new Uint8Array(worker.image)))}`}
                                        alt="Фото"
                                        sx={{ width: 120, height: 120 }}
                                    />
                                ) : (
                                    <Avatar sx={{ width: 120, height: 120, bgcolor: '#ccc', color: '#666' }}>
                                        📷
                                    </Avatar>
                                )}
                            </Box>

                            <Typography variant="h5" align="center" fontWeight="bold">
                                {workerName}
                            </Typography>

                            {/*<Typography variant="body2" align="center" color="text.secondary">*/}
                            {/*    Email: {worker.email || '—'}*/}
                            {/*</Typography>*/}

                            {/*<Typography variant="body2" align="center" color="text.secondary">*/}
                            {/*    Телефон: {worker.phoneNumber || '—'}*/}
                            {/*</Typography>*/}
                            <Typography variant="body2" align="center" color="text.secondary">
                                Возраст: {worker.age || '—'}
                            </Typography>

                            <Typography variant="body2" align="center" color="text.secondary">
                                Город: {resume.city || '—'}
                            </Typography>
                        </Stack>
                    </Paper>
                </Grid>

                {/* Основная информация о резюме */}
                <Grid item xs={12} md={8}>
                    <Paper elevation={3} sx={{
                        p: 3,
                        borderRadius: 3,
                        mb: 3
                    }}>
                        <Stack spacing={2}>
                            <Typography variant="h5" fontWeight="bold">
                                Должность: {resume.post || 'Junior Frontend Developer'}
                            </Typography>
                            <Divider />

                            <Box display="flex" justifyContent="space-between" flexWrap="wrap">
                                <Box>
                                    <Typography><strong>Опыт работы:</strong> {resume.experience} лет</Typography>
                                    <Typography><strong>Желаемая зарплата:</strong> {resume.wantedSalary} ₽</Typography>
                                    <Typography><strong>Образование:</strong> {getEducationName(resume.education_id)}</Typography>
                                    <Typography><strong>Дата добавления:</strong> {dayjs(resume.income_date).format('DD.MM.YYYY')}</Typography>
                                </Box>

                                <Box>
                                    <FormControl fullWidth sx={{ minWidth: 200 }}>
                                        <InputLabel id="vacancy-select-label">Выберите вакансию</InputLabel>
                                        <Select
                                            labelId="vacancy-select-label"
                                            value={selectedVacancyId}
                                            onChange={(e) => setSelectedVacancyId(e.target.value)}
                                            label="Выберите вакансию"
                                        >
                                            {vacancies.map(vacancy => {
                                                const salaryDisplay = vacancy.max_salary
                                                    ? `${vacancy.min_salary} - ${vacancy.max_salary}`
                                                    : vacancy.min_salary;

                                                return (
                                                    <MenuItem key={vacancy.id} value={vacancy.id}>
                                                        <Box>
                                                            <Typography variant="body2" fontWeight="bold">{vacancy.post}</Typography>
                                                            <Typography variant="caption">Зарплата: {salaryDisplay} ₽</Typography>
                                                            <Typography variant="caption">Образование: {getEducationName(vacancy.education_id)}</Typography>
                                                            <Typography variant="caption">Опыт: {vacancy.experience || '—'}</Typography>
                                                        </Box>
                                                    </MenuItem>
                                                );
                                            })}
                                        </Select>
                                    </FormControl>

                                    <Button
                                        variant="contained"
                                        color="primary"
                                        fullWidth
                                        disabled={!selectedVacancyId}
                                        onClick={handleRespond}
                                        sx={{ mt: 2, py: 1.2 }}
                                    >
                                        Откликнуться
                                    </Button>
                                </Box>
                            </Box>
                        </Stack>
                    </Paper>

                    {/* Активности */}
                    <Paper elevation={3} sx={{
                        p: 3,
                        borderRadius: 3,
                        mb: 3
                    }}>
                        <Typography variant="h6" fontWeight="bold" gutterBottom>
                            Фильтры
                        </Typography>
                        <Box display="flex" flexWrap="wrap" gap={1}>
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
                    </Paper>

                    {/* Полное описание */}
                    <Paper elevation={3} sx={{
                        p: 3,
                        borderRadius: 3,
                        mb: 3
                    }}>
                        <Typography variant="h6" fontWeight="bold" gutterBottom>
                            Описание
                        </Typography>
                        <Divider sx={{ mb: 2 }} />
                        <Typography variant="body1" whiteSpace="pre-line" sx={{ textAlign: 'left' }}>
                            {resume.skill}
                        </Typography>
                    </Paper>
                </Grid>
            </Grid>

            <Button variant="outlined" onClick={() => navigate(-1)} sx={{ mt: 2 }}>
                Назад
            </Button>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}