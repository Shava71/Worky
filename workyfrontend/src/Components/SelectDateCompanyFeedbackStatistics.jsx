import React, { useState } from 'react';
import {
    Container,
    Paper,
    Typography,
    Box,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
} from '@mui/material';
import {useNavigate} from "react-router-dom";


export default function SelectDateCompanyFeedbackStatistics() {
    const [startDate, setStartDate] = useState({ month: '', year: '' });
    const [endDate, setEndDate] = useState({ month: '', year: '' });
    const navigate = useNavigate();


    const months = [
        { value: 1, label: 'Январь' },
        { value: 2, label: 'Февраль' },
        { value: 3, label: 'Март' },
        { value: 4, label: 'Апрель' },
        { value: 5, label: 'Май' },
        { value: 6, label: 'Июнь' },
        { value: 7, label: 'Июль' },
        { value: 8, label: 'Август' },
        { value: 9, label: 'Сентябрь' },
        { value: 10, label: 'Октябрь' },
        { value: 11, label: 'Ноябрь' },
        { value: 12, label: 'Декабрь' },
    ];

    const years = Array.from({ length: 10 }, (_, i) => new Date().getFullYear() - 5 + i);

    const handleStartDateChange = (type, value) => {
        setStartDate(prev => ({ ...prev, [type]: value }));
    };

    const handleEndDateChange = (type, value) => {
        setEndDate(prev => ({ ...prev, [type]: value }));
    };

    const handleViewStats = () => {
        if (!startDate.month || !startDate.year || !endDate.month || !endDate.year) {
            alert('Выберите оба месяца и годы');
            return;
        }
        navigate(`/Statistics/Company/Feedback?start_year=${startDate.year}&start_month=${startDate.month}&end_year=${endDate.year}&end_month=${endDate.month}`);
    };

    return (
        <Container maxWidth="md" sx={{ py: 6 }}>
            <Paper elevation={3} sx={{ p: 4, borderRadius: 3 }}>
                <Typography variant="h5" fontWeight="bold" gutterBottom>
                    📊 Статистика за период
                </Typography>

                <Box display="flex" gap={2} mb={3}>
                    {/* Начало */}
                    <Box flex={1}>
                        <FormControl fullWidth>
                            <InputLabel id="month-start-label">Месяц начала</InputLabel>
                            <Select
                                labelId="month-start-label"
                                value={startDate.month}
                                onChange={(e) => handleStartDateChange('month', e.target.value)}
                                label="Месяц начала"
                            >
                                {months.map(m => (
                                    <MenuItem key={m.value} value={m.value}>{m.label}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                        <FormControl fullWidth sx={{ mt: 2 }}>
                            <InputLabel id="year-start-label">Год начала</InputLabel>
                            <Select
                                labelId="year-start-label"
                                value={startDate.year}
                                onChange={(e) => handleStartDateChange('year', e.target.value)}
                                label="Год начала"
                            >
                                {years.map(y => (
                                    <MenuItem key={y} value={y}>{y}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Box>

                    {/* Конец */}
                    <Box flex={1}>
                        <FormControl fullWidth>
                            <InputLabel id="month-end-label">Месяц окончания</InputLabel>
                            <Select
                                labelId="month-end-label"
                                value={endDate.month}
                                onChange={(e) => handleEndDateChange('month', e.target.value)}
                                label="Месяц окончания"
                            >
                                {months.map(m => (
                                    <MenuItem key={m.value} value={m.value}>{m.label}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                        <FormControl fullWidth sx={{ mt: 2 }}>
                            <InputLabel id="year-end-label">Год окончания</InputLabel>
                            <Select
                                labelId="year-end-label"
                                value={endDate.year}
                                onChange={(e) => handleEndDateChange('year', e.target.value)}
                                label="Год окончания"
                            >
                                {years.map(y => (
                                    <MenuItem key={y} value={y}>{y}</MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Box>
                </Box>

                <Button
                    variant="contained"
                    color="primary"
                    onClick={handleViewStats}
                    fullWidth
                    disabled={!startDate.month || !startDate.year || !endDate.month || !endDate.year}
                >
                    Показать статистику
                </Button>
            </Paper>
        </Container>
    );
}