import React, { useEffect, useState } from 'react';
import {
    Container,
    Typography,
    Paper,
    Box,
    Stack,
    Button,
    Divider,
    Snackbar,
    Alert,
    Grid, Chip,
} from '@mui/material';
import axios from 'axios';
import { useSearchParams } from 'react-router-dom';

export default function StatisticCompanyFeedback() {
    const [searchParams] = useSearchParams();
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

    // Получаем параметры из URL
    const startYear = searchParams.get('start_year');
    const startMonth = searchParams.get('start_month');
    const endYear = searchParams.get('end_year');
    const endMonth = searchParams.get('end_month');

    // Загрузка статистики по умолчанию за последний месяц
    useEffect(() => {
        const fetchStats = async () => {
            try {
                const token = localStorage.getItem('jwt');
                const response = await axios.get('https://localhost:7106/api/v1/Company/Statistics/json',  {
                    headers: { Authorization: `Bearer ${token}` },
                    params: {
                        start_year: startYear,
                        start_month: startMonth,
                        end_year: endYear,
                        end_month: endMonth,
                    }
                });
                setStats(response.data);
            } catch (err) {
                console.error('Ошибка загрузки статистики:', err);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить статистику',
                    severity: 'error'
                });
            } finally {
                setLoading(false);
            }
        };

        if (startYear && startMonth && endYear && endMonth) {
            fetchStats();
        }
    }, [startYear, startMonth, endYear, endMonth]);


    const handleDownloadPdf = async () => {
        try {
            const token = localStorage.getItem('jwt');

            const response = await axios.get(`https://localhost:7106/api/v1/Company/Statistics/pdf`,  {
                headers: {
                    Authorization: `Bearer ${token}`,
                },
                params: {
                    start_year: startYear,
                    start_month: startMonth,
                    end_year: endYear,
                    end_month: endMonth,
                },
                responseType: 'blob', // ВАЖНО: для скачивания файла
            });

            const contentDisposition = response.headers['content-disposition'];
            let filename = 'statistics.pdf';

            if (contentDisposition && contentDisposition.includes('filename=')) {
                const fileNameMatch = contentDisposition.match(/filename="?([^"]+)"?/);
                if (fileNameMatch.length > 1) {
                    filename = fileNameMatch[1];
                }
            }

            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', filename);
            document.body.appendChild(link);
            link.click();
            link.remove();

            setSnackbar({
                open: true,
                message: 'PDF-отчет успешно загружен!',
                severity: 'success',
            });
        } catch (error) {
            console.error('Ошибка при загрузке отчета:', error);
            setSnackbar({
                open: true,
                message: 'Не удалось загрузить PDF-отчет',
                severity: 'error',
            });
        }
    };

    if (loading) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <Typography variant="h6">Загрузка статистики...</Typography>
            </Box>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ py: 6 }}>
            <Paper elevation={3} sx={{ p: 4, borderRadius: 3, mb: 4 }}>
                <Typography variant="h5" align="center" gutterBottom fontWeight="bold">
                    📊 Статистика компании
                </Typography>
                <Divider sx={{ mb: 3 }} />
                <Grid container spacing={2}>
                    <Grid item xs={12} sm={6}>
                        <Typography><strong>Вакансии:</strong> {stats.vacancyCount}</Typography>
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Typography><strong>Отклики всего:</strong> {stats.totalFeedbacks}</Typography>
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Typography><strong>Принятые кандидаты:</strong> {stats.acceptedWorkers.length}</Typography>
                    </Grid>
                    <Grid item xs={12} sm={6}>
                        <Typography><strong>Среднее число откликов на вакансию:</strong> {stats.avgFeedbackPerVacancy}</Typography>
                    </Grid>
                    <Grid item xs={12}>
                        <Typography><strong>Период:</strong> {stats.period}</Typography>
                    </Grid>
                </Grid>
                <Button
                    variant="contained"
                    color="secondary"
                    fullWidth
                    sx={{ mt: 3 }}
                    onClick={handleDownloadPdf}
                >
                    Скачать PDF-отчет
                </Button>
            </Paper>

            {/* Список принятых сотрудников */}
            {stats.acceptedWorkers && stats.acceptedWorkers.length > 0 && (
                <>
                    <Typography variant="h6" gutterBottom>Принятые сотрудники</Typography>
                    <Grid container spacing={2}>
                        {stats.acceptedWorkers.map(worker => (
                            <Grid item xs={12} sm={6} md={4} key={worker.id}>
                                <Paper elevation={3} sx={{ p: 2, borderRadius: 2 }}>
                                    <Typography variant="subtitle1" fontWeight="bold">
                                        {`${worker.second_name} ${worker.first_name} ${worker.surname || ''}`}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        📧 Email: {worker.email}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        📞 Телефон: {worker.phone || 'Не указан'}
                                    </Typography>
                                    <Typography variant="body2" color="text.secondary">
                                        🎂 Возраст: {worker.age}
                                    </Typography>
                                </Paper>
                            </Grid>
                        ))}
                    </Grid>
                </>
            )}

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}