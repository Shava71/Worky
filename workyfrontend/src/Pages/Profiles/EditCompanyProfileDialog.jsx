import React, { useEffect, useState } from "react";
import {
    Dialog, DialogTitle, DialogContent, DialogActions,
    Button, TextField, Stack
} from "@mui/material";

import api from "../../api/myaxios.js";
import {API} from "../../api/routes.js";

export default function EditCompanyProfileDialog({
                                                     open,
                                                     onClose,
                                                     company,
                                                     userId,
                                                     onUpdated
                                                 }) {
    const [form, setForm] = useState({
        name: "",
        email: "",
        phoneNumber: "",
        website: "",
        latitude: "",
        longitude: ""
    });

    const [file, setFile] = useState(null);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        if (company) {
            setForm({
                name: company.name || "",
                email: company.email || "",
                phoneNumber: company.phoneNumber || "",
                website: company.website || "",
                latitude: company.latitude || "",
                longitude: company.longitude || ""
            });
        }
    }, [company]);

    const handleChange = (e) => {
        setForm(prev => ({
            ...prev,
            [e.target.name]: e.target.value
        }));
    };

    const handleFile = (e) => {
        setFile(e.target.files?.[0] || null);
    };

    const handleSubmit = async () => {
        try {
            setSaving(true);

            // 1. обновляем профиль компании
            await api.put(API.company.updateProfile(userId), {
                name: form.name,
                email: form.email,
                phoneNumber: form.phoneNumber,
                website: form.website,
                latitude: form.latitude,
                longitude: form.longitude
            });

            // 2. если есть фото — получаем presigned PUT и грузим в MinIO напрямую
            if (file) {
                const presign = await api.post(API.auth.uploadPhoto(userId));
                const presignedUrl = presign.data.url;
                // заменяем хост на localhost через /minio
                const url = presignedUrl.replace(/^http:\/\/minio:9000/, 'http://localhost:8080/minio');
                const webpFile = await convertToWebp(file);
                await fetch(url, {
                    method: "PUT",
                    // headers: {
                    //     "Content-Type": file.type
                    // },
                    body: webpFile
                });
            }

            onUpdated?.();
            onClose();
        } catch (e) {
            console.error("Ошибка обновления профиля:", e);
        } finally {
            setSaving(false);
        }
    };

    const convertToWebp = (file) => {
        return new Promise((resolve) => {

            const img = new Image();
            const reader = new FileReader();

            reader.onload = (e) => {
                img.src = e.target.result;
            };

            img.onload = () => {

                const canvas = document.createElement("canvas");
                canvas.width = img.width;
                canvas.height = img.height;

                const ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0);

                canvas.toBlob((blob) => {
                    resolve(blob);
                }, "image/webp", 0.9);

            };

            reader.readAsDataURL(file);
        });
    };

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
            <DialogTitle>Редактировать профиль компании</DialogTitle>

            <DialogContent>
                <Stack spacing={2} mt={1}>
                    <TextField label="Название" name="name" value={form.name} onChange={handleChange} fullWidth />
                    <TextField label="Email" name="email" value={form.email} onChange={handleChange} fullWidth />
                    <TextField label="Телефон" name="phoneNumber" value={form.phoneNumber} onChange={handleChange} fullWidth />
                    <TextField label="Сайт" name="website" value={form.website} onChange={handleChange} fullWidth />
                    <TextField label="Latitude" name="latitude" value={form.latitude} onChange={handleChange} />
                    <TextField label="Longitude" name="longitude" value={form.longitude} onChange={handleChange} />

                    <Button variant="outlined" component="label">
                        Загрузить фото
                        <input hidden type="file" accept="image/*" onChange={handleFile} />
                    </Button>
                </Stack>
            </DialogContent>

            <DialogActions>
                <Button onClick={onClose}>Отмена</Button>
                <Button variant="contained" onClick={handleSubmit} disabled={saving}>
                    Сохранить
                </Button>
            </DialogActions>
        </Dialog>
    );
}