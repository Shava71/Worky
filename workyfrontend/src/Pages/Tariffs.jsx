import React, { useEffect, useState } from 'react';
import {
    Box,
    Typography,
    Paper,
    Button,
    Stack,
    Divider,
    Chip,
    Snackbar,
    Alert,
    Modal,
    TextField,
    Tabs,
    Tab,
    Card,
    CardContent,
    Table,
    TableBody,
    TableCell,
    TableRow,
    TableContainer,
} from '@mui/material';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

export default function Tariffs() {
    const [tariffs, setTariffs] = useState([]);
    const [loading, setLoading] = useState(true);
    const [noDeal, setNoDeal] = useState(false);
    const [modalOpen, setModalOpen] = useState(false);
    const [selectedTariff, setSelectedTariff] = useState(null);
    const [months, setMonths] = useState(1);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [tabValue, setTabValue] = useState(0);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const navigate = useNavigate();

    // Загрузка данных
    useEffect(() => {
        const fetchData = async () => {
            try {
                const token = localStorage.getItem('jwt');
                const role = localStorage.getItem('role');
                let isAuthorized = false;

                // ВСЕГДА запрашиваем тарифы — они анонимные
                const tariffsResponse = await axios.get(
                    'https://localhost:7106/api/v1/Company/Tarrif'
                );
                const tariffsData = tariffsResponse.data.tarrifs || [];

                // Если есть токен — проверяем профиль и договор
                if (token && role == 'Company') {
                    const profileResponse = await axios.get(
                        'https://localhost:7106/api/v1/Company/GetProfile',
                        {
                            headers: { Authorization: `Bearer ${token}` },
                        }
                    );

                    const deals = profileResponse.data.deals || [];
                    const today = new Date();
                    const activeDeal = deals.find(deal =>
                        new Date(deal.date_start) <= new Date(today.setDate(today.getDate() + 1)) &&
                        new Date(deal.date_end) >= new Date(today.setDate(today.getDate() - 1))
                    );

                    if (!activeDeal) {
                        setNoDeal(true);
                    }

                    isAuthorized = true;
                }

                setTariffs(tariffsData);
                setIsAuthenticated(isAuthorized);
                setLoading(false);
            } catch (err) {
                console.error('Ошибка при загрузке данных:', err);
                setTariffs([]);
                setLoading(false);
                setIsAuthenticated(false);
            }
        };

        fetchData();
    }, []);

    // Обработка подключения тарифа
    const handleBuyTariff = async () => {
        try {
            const token = localStorage.getItem('jwt');
            if (!token || !selectedTariff) return;

            await axios.post(
                'https://localhost:7106/api/v1/Company/MakeDeal',
                { tarrif_id: selectedTariff.id, countMonth: months },
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'application/json',
                    },
                }
            );

            setSnackbar({
                open: true,
                message: 'Тариф успешно подключен!',
                severity: 'success',
            });

            setTimeout(() => navigate('/Company/Profile'), 1500);
            setModalOpen(false);
        } catch {
            setSnackbar({
                open: true,
                message: 'Ошибка при подключении тарифа',
                severity: 'error',
            });
        }
    };

    // Закрытие уведомления
    const handleCloseSnackbar = () => {
        setSnackbar((prev) => ({ ...prev, open: false }));
    };

    // Открытие модального окна
    const handleOpenModal = (tariff) => {
        setSelectedTariff(tariff);
        setModalOpen(true);
    };

    // Смена табов
    const handleChangeTab = (event, newValue) => {
        setTabValue(newValue);
    };

    if (loading) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <Typography variant="h5">Загрузка тарифов...</Typography>
            </Box>
        );
    }

    return (
        <Box sx={{ py: 6, px: 2, maxWidth: 1200, mx: 'auto' }}>
            {/* Промо-баннер */}
            <Paper
                elevation={3}
                sx={{
                    p: 4,
                    mb: 5,
                    bgcolor: 'primary.light',
                    color: 'white',
                    borderRadius: 3,
                    textAlign: 'center',
                }}
            >
                <Typography variant="h5" fontWeight="bold">
                    🚀 Выберите тариф и начните размещать вакансии уже сегодня!
                </Typography>
                <Typography variant="body1" sx={{ mt: 1, opacity: 0.9 }}>
                    Подключите тариф всего за пару кликов — без скрытых платежей.
                </Typography>
            </Paper>

            {/* Табы */}
            <Box sx={{ borderBottom: 1, borderColor: 'divider', mb: 3 }}>
                <Tabs value={tabValue} onChange={handleChangeTab} centered>
                    <Tab label="Тарифы" />
                    <Tab label="Сравнение" />
                </Tabs>
            </Box>

            {/* Контент по табам */}
            {tabValue === 0 && (
                <Box
                    sx={{
                        display: 'grid',
                        gridTemplateColumns: { xs: '1fr', sm: 'repeat(2, 1fr)' },
                        gap: 4,
                    }}
                >
                    {tariffs.map((tariff) => (
                        <Paper
                            key={tariff.id}
                            elevation={6}
                            sx={{
                                p: 4,
                                borderRadius: 3,
                                height: 'auto',
                                display: 'flex',
                                flexDirection: 'column',
                                justifyContent: 'space-between',
                                transition: 'transform 0.3s ease',
                                '&:hover': { transform: 'scale(1.02)' },
                                boxShadow: 3,
                            }}
                        >
                            <Box>
                                <Typography variant="h5" fontWeight="bold" align="center" gutterBottom>
                                    {tariff.name}
                                </Typography>
                                <Divider sx={{ mb: 2 }} />
                                <Typography variant="body1" align="center" sx={{ mb: 2 }}>
                                    {tariff.description}
                                </Typography>
                                <Chip
                                    label={`${tariff.vacancy_count} вакансий`}
                                    color="primary"
                                    size="medium"
                                    sx={{ display: 'block', mx: 'auto', mb: 2 }}
                                />
                                <Typography
                                    variant="h5"
                                    align="center"
                                    fontWeight="bold"
                                    color="primary"
                                    gutterBottom
                                >
                                    {tariff.price} ₽ / мес.
                                </Typography>
                            </Box>

                            {isAuthenticated && noDeal && (
                                <Button
                                    variant="contained"
                                    color="primary"
                                    onClick={() => handleOpenModal(tariff)}
                                    sx={{
                                        mt: 3,
                                        py: 1.2,
                                        borderRadius: 2,
                                        fontWeight: 'bold',
                                        alignSelf: 'center',
                                        width: 'fit-content',
                                        '&:hover': { bgcolor: 'primary.dark' },
                                    }}
                                >
                                    Подключить тариф
                                </Button>
                            )}

                            {!isAuthenticated && (
                                <Typography
                                    variant="body2"
                                    align="center"
                                    color="text.secondary"
                                    sx={{ mt: 2 }}
                                >
                                    Войдите в качестве компании, чтобы подключить тариф
                                </Typography>
                            )}
                        </Paper>
                    ))}
                </Box>
            )}

            {tabValue === 1 && (
                <TableContainer component={Paper} elevation={3}>
                    <Table>
                        <TableBody>
                            <TableRow>
                                <TableCell>Название</TableCell>
                                {tariffs.map((t) => (
                                    <TableCell key={t.id} align="center">
                                        <strong>{t.name}</strong>
                                    </TableCell>
                                ))}
                            </TableRow>
                            <TableRow>
                                <TableCell>Количество вакансий</TableCell>
                                {tariffs.map((t) => (
                                    <TableCell key={t.id} align="center">
                                        {t.vacancy_count}
                                    </TableCell>
                                ))}
                            </TableRow>
                            <TableRow>
                                <TableCell>Цена за месяц</TableCell>
                                {tariffs.map((t) => (
                                    <TableCell key={t.id} align="center">
                                        {t.price} ₽
                                    </TableCell>
                                ))}
                            </TableRow>
                            {isAuthenticated && (
                                <TableRow>
                                    <TableCell>Действия</TableCell>
                                    {tariffs.map((t) => (
                                        <TableCell key={t.id} align="center">
                                            {noDeal ? (
                                                <Button
                                                    size="small"
                                                    variant="outlined"
                                                    onClick={() => handleOpenModal(t)}
                                                >
                                                    Подключить
                                                </Button>
                                            ) : (
                                                <Typography variant="body2" color="text.secondary">
                                                    Действующий тариф
                                                </Typography>
                                            )}
                                        </TableCell>
                                    ))}
                                </TableRow>
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}

            {/* Модальное окно */}
            <Modal open={modalOpen} onClose={() => setModalOpen(false)}>
                <Card
                    sx={{
                        position: 'absolute',
                        top: '50%',
                        left: '50%',
                        transform: 'translate(-50%, -50%)',
                        width: 400,
                        bgcolor: 'background.paper',
                        borderRadius: 2,
                        boxShadow: 24,
                        p: 4,
                    }}
                >
                    <CardContent>
                        <Typography variant="h6" gutterBottom>
                            Подключение тарифа: {selectedTariff?.name}
                        </Typography>
                        <TextField
                            label="Количество месяцев"
                            type="number"
                            fullWidth
                            value={months}
                            onChange={(e) => setMonths(parseInt(e.target.value))}
                            inputProps={{ min: 1 }}
                            sx={{ mb: 3 }}
                        />
                        <Stack direction="row" spacing={2}>
                            <Button variant="contained" color="primary" onClick={handleBuyTariff}>
                                Подключить
                            </Button>
                            <Button variant="outlined" onClick={() => setModalOpen(false)}>
                                Отмена
                            </Button>
                        </Stack>
                    </CardContent>
                </Card>
            </Modal>

            {/* Сообщение о наличии активного договора */}
            {!noDeal && isAuthenticated && (
                <Box sx={{ mt: 6, textAlign: 'center' }}>
                    <Typography variant="body1" color="text.secondary">
                        У вас уже есть активный тариф. Чтобы изменить его, завершите текущий договор.
                    </Typography>
                </Box>
            )}

            {/* Snackbar для уведомлений */}
            <Snackbar
                open={snackbar.open}
                autoHideDuration={3000}
                onClose={handleCloseSnackbar}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
            >
                <Alert onClose={handleCloseSnackbar} severity={snackbar.severity} sx={{ width: '100%' }}>
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </Box>
    );
}