import React, { useEffect, useState } from 'react';
import {
    Box,
    Typography,
    Paper,
    Divider,
    Button,
    Stack,
    Skeleton,
    Pagination,
    Link,
    Avatar,
    Chip,
} from '@mui/material';
import DownloadIcon from '@mui/icons-material/Download';
import dayjs from 'dayjs';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import SelectDateCompanyFeedbackStatistics from "../../Components/SelectDateCompanyFeedbackStatistics.jsx";
import api from '../../api/axios';
import { API } from '../../api/routes';

const ITEMS_PER_PAGE = 5;

export default function CompanyProfile() {
    const [loading, setLoading] = useState(true);
    const [currentPage, setCurrentPage] = useState(1);
    const [paginatedDeals, setPaginatedDeals] = useState([]);
    const [companyData, setCompanyData] = useState(null);
    const [tariffs, setTariffs] = useState({});
    const navigate = useNavigate();

    // Загрузка данных профиля и тарифов
    useEffect(() => {
        const fetchCompanyData = async () => {
            try {
                const response = await api.get(API.company.profile);
                setCompanyData(response.data);
                setLoading(false);
                const tariffResponse = await api.get(API.deal.tariffs);

                const tariffMap = {};
                tariffResponse.data.tarrifs.forEach(t => {
                    tariffMap[t.id] = t;
                });
                setTariffs(tariffMap);
            } catch (error) {
                console.error('Ошибка при загрузке данных компании:', error);
                setLoading(false);
            }
        };
        fetchCompanyData();
    }, []);

    const today = dayjs();
    const deals = companyData?.deals || [];
    const company = companyData?.company || {};

    const currentDeal = deals.find(deal =>
        dayjs(deal.date_start).isBefore(today.add(1, 'day')) &&
        dayjs(deal.date_end).isAfter(today.subtract(1, 'day'))
    );

    useEffect(() => {
        if (deals.length) {
            const start = (currentPage - 1) * ITEMS_PER_PAGE;
            const end = start + ITEMS_PER_PAGE;
            setPaginatedDeals(deals.slice(start, end));
        }
    }, [deals, currentPage]);

    const handleDownloadReceipt = async (dealId) => {
        try {
            const token = localStorage.getItem('jwt');
            const response = await axios.get(`https://localhost:7106/api/v1/Company/receipt/${dealId}`, {
                headers: { Authorization: `Bearer ${token}` },
                responseType: 'blob',
            });
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `receipt_${dealId}.pdf`);
            document.body.appendChild(link);
            link.click();
            window.URL.revokeObjectURL(url);
        } catch (error) {
            console.error('Ошибка при скачивании чека:', error);
        }
    };

    if (loading) {
        return (
            <Box sx={{ mt: 10 }}>
                <Skeleton variant="text" width={300} height={40} />
                <Skeleton variant="rectangular" height={200} sx={{ mt: 2 }} />
                <Skeleton variant="rectangular" height={100} sx={{ mt: 2 }} />
                <Skeleton variant="rectangular" height={100} sx={{ mt: 2 }} />
            </Box>
        );
    }

    return (
        <Box sx={{ mt: 8, px: 2, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Box maxWidth="md" width="100%">
                <Typography variant="h4" gutterBottom fontWeight="bold">
                    🏢 Профиль компании: {company.name}
                </Typography>

                {/* Основная информация */}
                <Paper elevation={6} sx={{
                    p: 4,
                    mb: 4,
                    borderRadius: 4,
                    bgcolor: '#ffffff',
                    transition: 'transform 0.2s ease',
                    '&:hover': { transform: 'translateY(-4px)' }
                }}>
                    <Stack spacing={2}>
                        <Stack direction="row" spacing={2} alignItems="center">
                            <Avatar sx={{ bgcolor: 'primary.main' }}>{company.name?.charAt(0)}</Avatar>
                            <Typography variant="h6">Основная информация</Typography>
                        </Stack>
                        <Divider />
                        <Typography><strong>Email:</strong> {company.email}</Typography>
                        <Typography><strong>Телефон:</strong> {company.phoneNumber}</Typography>
                        <Typography>
                            <strong>Сайт:</strong>{' '}
                            <Link href={company.website} target="_blank" rel="noopener">
                                {company.website}
                            </Link>
                        </Typography>
                        <Typography><strong>Координаты:</strong> {company.latitude}, {company.longitude}</Typography>
                    </Stack>
                </Paper>

                {/* Текущий договор или предложение подключить тариф */}
                {currentDeal ? (
                    (() => {
                        const tariff = tariffs[currentDeal.tariff_id];
                        return (
                            <Paper elevation={4} sx={{
                                p: 4,
                                mb: 4,
                                borderRadius: 4,
                                bgcolor: '#f9f9f9',
                                transition: 'transform 0.2s ease',
                                '&:hover': { transform: 'translateY(-4px)' }
                            }}>
                                <Typography variant="h6" fontWeight="bold" gutterBottom>
                                    🔁 Текущий активный договор
                                </Typography>
                                <Divider sx={{ mb: 2 }} />
                                <Typography><strong>Сумма:</strong> {currentDeal.sum} ₽</Typography>
                                <Typography>
                                    <strong>Период:</strong> {dayjs(currentDeal.date_start).format('DD.MM.YYYY')} — {dayjs(currentDeal.date_end).format('DD.MM.YYYY')}
                                </Typography>
                                <Typography><strong>Длительность:</strong> {currentDeal.duration_month} мес.</Typography>
                                {tariff && (
                                    <>
                                        <Typography><strong>Тариф:</strong> {tariff.name}</Typography>
                                        <Typography><strong>Кол-во вакансий:</strong> {tariff.vacancy_count}</Typography>
                                    </>
                                )}
                                <Chip
                                    label={currentDeal.status ? "Оплачен" : "Не оплачен"}
                                    color={currentDeal.status ? "success" : "error"}
                                    size="small"
                                    sx={{ mt: 1 }}
                                />
                                <Button
                                    variant="contained"
                                    startIcon={<DownloadIcon />}
                                    onClick={() => handleDownloadReceipt(currentDeal.id)}
                                    sx={{
                                        mt: 2,
                                        borderRadius: 3,
                                        px: 3,
                                        py: 1,
                                        bgcolor: 'primary.main',
                                        '&:hover': { bgcolor: 'primary.dark' }
                                    }}
                                >
                                    Скачать чек
                                </Button>
                            </Paper>
                        );
                    })()
                ) : (
                    <Paper elevation={4} sx={{
                        p: 4,
                        mb: 4,
                        borderRadius: 4,
                        bgcolor: '#fff3cd',
                        borderLeft: 5,
                        borderColor: 'warning.main',
                        transition: 'transform 0.2s ease',
                        '&:hover': { transform: 'translateY(-4px)' }
                    }}>
                        <Typography variant="h6" fontWeight="bold" gutterBottom color="warning.dark">
                            ⚠️ Нет активного договора
                        </Typography>
                        <Typography variant="body1" sx={{ mb: 2 }} color="text.secondary">
                            Чтобы выкладывать вакансии, необходимо подключить тариф.
                        </Typography>
                        <Button
                            variant="contained"
                            color="warning"
                            onClick={() => navigate('/Tariffs')}
                            sx={{
                                borderRadius: 3,
                                px: 3,
                                py: 1.2,
                                fontWeight: 'bold',
                                '&:hover': { bgcolor: 'warning.dark' }
                            }}
                        >
                            Подключить тариф
                        </Button>
                    </Paper>
                )}

                {/* История договоров */}
                <Typography variant="h6" fontWeight="bold" gutterBottom>
                    📜 История договоров
                </Typography>
                <Stack spacing={3} sx={{ mb: 4 }}>
                    {paginatedDeals.map((deal) => {
                        const tariff = tariffs[deal.tariff_id];
                        return (
                            <Paper key={deal.id} elevation={3} sx={{
                                p: 3,
                                borderRadius: 3,
                                bgcolor: '#fff',
                                transition: 'transform 0.2s ease',
                                '&:hover': { transform: 'translateX(5px)' }
                            }}>
                                <Stack spacing={1}>
                                    <Typography variant="subtitle1" fontWeight="bold">
                                        Договор #{deal.id}
                                    </Typography>
                                    <Typography><strong>Сумма:</strong> {deal.sum} ₽</Typography>
                                    <Typography>
                                        <strong>Период:</strong> {dayjs(deal.date_start).format('DD.MM.YYYY')} — {dayjs(deal.date_end).format('DD.MM.YYYY')}
                                    </Typography>
                                    <Typography><strong>Длительность:</strong> {deal.duration_month} мес.</Typography>
                                    {tariff && (
                                        <>
                                            <Typography><strong>Тариф:</strong> {tariff.name}</Typography>
                                            <Typography><strong>Кол-во вакансий:</strong> {tariff.vacancy_count}</Typography>
                                        </>
                                    )}
                                </Stack>

                                {/* Центрированные элементы: статус и кнопка */}
                                <Stack
                                    spacing={2}
                                    alignItems="center"
                                    justifyContent="center"
                                    sx={{ mt: 2 }}
                                >
                                    <Chip
                                        label={deal.status ? "Оплачен" : "Не оплачен"}
                                        color={deal.status ? "success" : "error"}
                                        size="medium"
                                    />
                                    <Button
                                        variant="outlined"
                                        startIcon={<DownloadIcon />}
                                        onClick={() => handleDownloadReceipt(deal.id)}
                                        sx={{
                                            borderRadius: 3,
                                            borderColor: 'primary.main',
                                            color: 'primary.main',
                                            '&:hover': {
                                                bgcolor: 'primary.light',
                                                borderColor: 'primary.dark'
                                            }
                                        }}
                                    >
                                        Скачать чек
                                    </Button>
                                </Stack>
                            </Paper>
                        );
                    })}
                </Stack>

                {/* Пагинация */}
                <Box sx={{ display: 'flex', justifyContent: 'center', my: 4 }}>
                    <Pagination
                        count={Math.ceil(deals.length / ITEMS_PER_PAGE)}
                        page={currentPage}
                        onChange={(e, value) => setCurrentPage(value)}
                        color="primary"
                        shape="rounded"
                        size="large"
                    />
                </Box>
                <SelectDateCompanyFeedbackStatistics></SelectDateCompanyFeedbackStatistics>
            </Box>
        </Box>
    );
}