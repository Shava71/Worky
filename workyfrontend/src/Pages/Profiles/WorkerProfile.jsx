import React, { useEffect, useState } from 'react';
import {
    Box,
    Typography,
    Paper,
    Divider,
    Avatar,
    Button,
    Stack,
    Skeleton,
    TextField,
    Snackbar,
    Alert
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import dayjs from 'dayjs';
import api from '../../api/myaxios';
import { API } from '../../api/routes';
import EditWorkerProfileDialog from './EditWorkerProfileDialog.jsx';

export default function WorkerProfile() {
    const [loading, setLoading] = useState(true);
    const [profile, setProfile] = useState(null);
    const [photoUrl, setPhotoUrl] = useState(null);
    const [editOpen, setEditOpen] = useState(false);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

    const userId = profile?.worker?.id || null;
    // -----------------------------
    // Загрузка профиля
    // -----------------------------
    const fetchProfile = async () => {
        setLoading(true);
        try {
            const res = await api.get(API.worker.profile);
            setProfile(res.data);


            setLoading(false);
        } catch (err) {
            console.error('Ошибка при загрузке профиля:', err);
            setSnackbar({ open: true, message: 'Не удалось загрузить профиль', severity: 'error' });
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProfile();
    }, []);

    useEffect(() => {

        const loadPhoto = async () => {

            if (!userId) return;

            try {

                const response = await api.get(API.auth.getPhoto(userId));
                const url = response.data.url;
                const urlGet = url.replace(/^http:\/\/minio:9000/, 'http://localhost:8080/minio');
                setPhotoUrl(urlGet);

            } catch {

                setPhotoUrl(null);

            }
        };

        loadPhoto();

    }, [userId]);

    if (loading) {
        return (
            <Box sx={{ mt: 10, px: 2 }}>
                <Skeleton variant="circular" width={80} height={80} />
                <Skeleton variant="text" width={300} height={40} sx={{ mt: 2 }} />
                <Skeleton variant="rectangular" height={200} sx={{ mt: 2 }} />
            </Box>
        );
    }

    if (!profile) {
        return (
            <Box sx={{ mt: 10, textAlign: 'center' }}>
                <Typography variant="h6">Профиль не найден</Typography>
            </Box>
        );
    }

    const { worker, UserResponse } = profile;

    return (
        <Box sx={{ mt: 8, px: 2, display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
            <Box maxWidth="md" width="100%">
                {/* Заголовок */}
                <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
                    <Typography variant="h4" fontWeight="bold">
                        👤 Профиль соискателя: {worker.first_name} {worker.second_name} {worker.surname}
                    </Typography>

                    <Button
                        variant="outlined"
                        startIcon={<EditIcon />}
                        onClick={() => setEditOpen(true)}
                    >
                        Редактировать
                    </Button>
                </Stack>

                {/* Основная информация */}
                <Paper elevation={6} sx={{ p: 4, mb: 4, borderRadius: 4, bgcolor: '#ffffff' }}>
                    <Stack spacing={2} direction="row" alignItems="center">
                        <Avatar
                            src={photoUrl || undefined}
                            sx={{ width: 80, height: 80, bgcolor: 'primary.main' }}
                        >
                            {!photoUrl && worker.first_name?.charAt(0)}
                        </Avatar>

                        <Box>
                            <Typography variant="h6">Основная информация</Typography>
                            <Divider sx={{ my: 1 }} />

                            <Typography>
                                <strong>ФИО:</strong> {worker.first_name} {worker.second_name} {worker.surname}
                            </Typography>

                            <Typography>
                                <strong>Дата рождения:</strong> {dayjs(worker.birthday).format('DD.MM.YYYY')}
                            </Typography>

                            <Typography>
                                <strong>Возраст:</strong> {worker.age ?? '—'} лет
                            </Typography>

                            {/*<Typography>*/}
                            {/*    <strong>Email:</strong> {worker.email || UserResponse?.Email || '—'}*/}
                            {/*</Typography>*/}

                            {/*<Typography>*/}
                            {/*    <strong>Телефон:</strong> {worker.phone || UserResponse?.PhoneNumber || '—'}*/}
                            {/*</Typography>*/}
                        </Box>
                    </Stack>
                </Paper>

            </Box>

            {/* Диалог редактирования */}
            <EditWorkerProfileDialog
                open={editOpen}
                onClose={() => setEditOpen(false)}
                profile={profile}
                onUpdated={fetchProfile}
            />

            {/* Snackbar */}
            <Snackbar
                open={snackbar.open}
                autoHideDuration={3000}
                onClose={() => setSnackbar(prev => ({ ...prev, open: false }))}
            >
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Box>
    );
}