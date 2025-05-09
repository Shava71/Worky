import React, { useState } from 'react';
import {
    Container,
    Paper,
    Typography,
    Box,
    TextField,
    Button,
    Stack,
    Snackbar,
    Alert, Divider
} from '@mui/material';
import axios from 'axios';
import YandexMapInput from "../../Components/YandexMapInput.jsx";


export default function CompanyRegister() {
    const [formData, setFormData] = useState({
        userName: '',
        email: '',
        phoneNumber: '',
        passwordHash: '',
        role: 'Company',
        name: '',
        email_info: '',
        phone_info: '',
        website: '',
    });
    const [latitude, setLatitude] = useState('');
    const [longitude, setLongitude] = useState('');

    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

    const handleChange = (e) => {
        setFormData(prev => ({
            ...prev,
            [e.target.name]: e.target.value
        }));
    };

    const handleCoordinatesChange = (lat, lng) => {
        setLatitude(lat.toString());
        setLongitude(lng.toString());
    };

    const handleSubmit = async () => {
        try {
            await axios.post('https://localhost:7106/api/v1/Authorization/Register', {
                ...formData,
                latitude,
                longitude
            }, {
                headers: { 'Content-Type': 'application/json' },
            });

            setSnackbar({
                open: true,
                message: 'Регистрация успешна!',
                severity: 'success'
            });

            setTimeout(() => {
                window.location.href = '/Login';
            }, 1500);
        } catch (err) {
            console.error('Ошибка при регистрации:', err.response?.data || err.message);
            setSnackbar({
                open: true,
                message: 'Не удалось зарегистрироваться',
                severity: 'error'
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar(prev => ({ ...prev, open: false }));
    };

    return (
        <Container maxWidth="md" sx={{ py: 6 }}>
            <Paper elevation={3} sx={{ p: 4, borderRadius: 3 }}>
                <Typography variant="h5" fontWeight="bold" gutterBottom align="center">
                    🏢 Регистрация компании
                </Typography>
                <Divider sx={{ mb: 3 }} />

                <Stack spacing={2}>
                    {/* Основные поля */}
                    <TextField label="Логин" name="userName" value={formData.userName} onChange={handleChange} fullWidth />
                    <TextField label="Email" name="email" value={formData.email} onChange={handleChange} fullWidth />
                    <TextField label="Номер телефона" name="phoneNumber" value={formData.phoneNumber} onChange={handleChange} fullWidth />
                    <TextField label="Пароль" name="passwordHash" type="password" value={formData.passwordHash} onChange={handleChange} fullWidth />

                    {/* Информация о компании */}
                    <TextField label="Название компании" name="name" value={formData.name} onChange={handleChange} fullWidth />
                    <TextField label="Email компании" name="email_info" value={formData.email_info} onChange={handleChange} fullWidth />
                    <TextField label="Телефон компании" name="phone_info" value={formData.phone_info} onChange={handleChange} fullWidth />
                    <TextField label="Сайт компании" name="website" value={formData.website} onChange={handleChange} fullWidth />

                    {/* Карта */}
                    <Box sx={{ mt: 2 }}>
                        <Typography variant="body2" gutterBottom>
                            Выберите офис на карте
                        </Typography>
                        <YandexMapInput onCoordinatesChange={handleCoordinatesChange} />
                    </Box>

                    {/* Скрытые поля широты и долготы */}
                    <TextField label="Широта" value={latitude} disabled fullWidth />
                    <TextField label="Долгота" value={longitude} disabled fullWidth />

                    <Button variant="contained" color="primary" onClick={handleSubmit} fullWidth sx={{ mt: 2 }}>
                        Зарегистрироваться
                    </Button>
                </Stack>
            </Paper>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={handleCloseSnackbar}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}