import { useNavigate } from "react-router-dom";
import { Button, Container, Box, Stack, Typography, Paper } from "@mui/material";

export default function HomeIndex() {
    const navigate = useNavigate();

    return (
        <Container maxWidth="md" sx={{ mt: 10 }}>
            <Paper elevation={6} sx={{ borderRadius: 4, p: 6, bgcolor: '#f5f5f5' }}>
                <Stack spacing={4} alignItems="center">
                    <Typography variant="h4" fontWeight="bold" gutterBottom>
                        Добро пожаловать в Worky!
                    </Typography>
                    <Typography variant="subtitle1" color="text.secondary" textAlign="center">
                        Выберите, кто вы: соискатель или компания, чтобы начать работу с платформой.
                    </Typography>

                    <Stack direction={{ xs: 'column', sm: 'row' }} spacing={3} width="100%">
                        <Box
                            sx={{
                                flex: 1,
                                transition: 'transform 0.3s',
                                ':hover': {
                                    transform: 'scale(1.05)',
                                },
                            }}
                        >
                            <Button
                                fullWidth
                                variant="contained"
                                size="large"
                                onClick={() => navigate('/WorkerRegister')}
                                sx={{
                                    py: 2,
                                    borderRadius: 3,
                                    fontSize: '1.2rem',
                                    bgcolor: 'primary.main',
                                    ':hover': {
                                        bgcolor: 'primary.dark',
                                    },
                                }}
                            >
                                Я Соискатель
                            </Button>
                        </Box>

                        <Box
                            sx={{
                                flex: 1,
                                transition: 'transform 0.3s',
                                ':hover': {
                                    transform: 'scale(1.05)',
                                },
                            }}
                        >
                            <Button
                                fullWidth
                                variant="contained"
                                size="large"
                                onClick={() => navigate('/CompanyRegister')}
                                sx={{
                                    py: 2,
                                    borderRadius: 3,
                                    fontSize: '1.2rem',
                                    bgcolor: 'secondary.main',
                                    ':hover': {
                                        bgcolor: 'secondary.dark',
                                    },
                                }}
                            >
                                Я Компания
                            </Button>
                        </Box>
                    </Stack>

                    <Typography variant="body2" color="text.secondary">
                        Уже есть аккаунт?
                    </Typography>

                    <Button
                        variant="outlined"
                        onClick={() => navigate('/Login')}
                        sx={{
                            fontSize: '1.1rem',
                            borderRadius: 3,
                            px: 4,
                            py: 1.5,
                            transition: '0.3s',
                            ':hover': {
                                borderColor: 'primary.main',
                                bgcolor: 'primary.light',
                            },
                        }}
                    >
                        Войти в систему
                    </Button>
                </Stack>
            </Paper>
        </Container>
    );
}