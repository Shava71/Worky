import React, { useState, useRef } from 'react';
import {
    YMaps,
    Map,
    Placemark,
    GeolocationControl
} from '@pbe/react-yandex-maps';
import {
    Box,
    TextField,
    InputAdornment,
    IconButton
} from '@mui/material';
import SearchIcon from '@mui/icons-material/Search';

export default function YandexMapInput({ onCoordinatesChange }) {
    const [mapCenter, setMapCenter] = useState([55.76, 37.64]);
    const [coordinates, setCoordinates] = useState(null);
    const [searchQuery, setSearchQuery] = useState('');
    const mapRef = useRef(null);

    const apiKey = '7604e5c6-d935-42b4-b82f-6eb56341706e';

    const handleMapClick = async (event) => {
        const coords = event.get('coords');
        setCoordinates(coords);
        onCoordinatesChange(coords[0], coords[1]);
        setMapCenter(coords);

        // Обратный геокодинг: координаты → адрес
        try {
            const response = await fetch(
                `https://geocode-maps.yandex.ru/1.x/?apikey=${apiKey}&format=json&geocode=${coords[1]},${coords[0]}`
            );
            const data = await response.json();
            const geoObject = data.response.GeoObjectCollection.featureMember[0]?.GeoObject;
            if (geoObject) {
                setSearchQuery(geoObject.name + ', ' + geoObject.description);
            }
        } catch (error) {
            console.error('Reverse geocoding error:', error);
        }
    };

    const handleSearch = async () => {
        if (!searchQuery.trim()) return;
        const url = `https://geocode-maps.yandex.ru/1.x/?apikey=${apiKey}&format=json&geocode=${encodeURIComponent(searchQuery)}`;

        try {
            const res = await fetch(url);
            const data = await res.json();
            const geoObject = data.response.GeoObjectCollection.featureMember[0]?.GeoObject;
            if (geoObject) {
                const [lon, lat] = geoObject.Point.pos.split(' ').map(Number);
                const newCoords = [lat, lon];
                setCoordinates(newCoords);
                setMapCenter(newCoords);
                onCoordinatesChange(lat, lon);
                mapRef.current.setCenter(newCoords, 14);
            }
        } catch (error) {
            console.error('Geocoding error:', error);
        }
    };

    return (
        <YMaps query={{ apikey: apiKey }}>
            <Box display="flex" flexDirection="column" gap={1}>
                <TextField
                    label="Введите адрес"
                    variant="outlined"
                    fullWidth
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleSearch()}
                    InputProps={{
                        endAdornment: (
                            <InputAdornment position="end">
                                <IconButton onClick={handleSearch} edge="end">
                                    <SearchIcon />
                                </IconButton>
                            </InputAdornment>
                        ),
                    }}
                />
                <div style={{ width: '100%', height: '400px', borderRadius: '2%', overflow: 'hidden' }}>
                    <Map
                        defaultState={{
                            center: mapCenter,
                            zoom: 10,
                            controls: ['zoomControl', 'fullscreenControl']
                        }}
                        onClick={handleMapClick}
                        modules={['control.ZoomControl', 'control.FullscreenControl']}
                        style={{ width: '100%', height: '100%' }}
                        instanceRef={mapRef}
                    >
                        {coordinates && <Placemark geometry={coordinates} />}
                        <GeolocationControl options={{ float: "left" }} />
                    </Map>
                </div>
            </Box>
        </YMaps>
    );
}