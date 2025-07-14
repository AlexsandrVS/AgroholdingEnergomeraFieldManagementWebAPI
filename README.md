# AgroholdingEnergomeraFieldManagementWebAPI

1. Получить все поля

Запрос:
GET /api/fields HTTP/1.1
Host: localhost:6347

Ответ 200 OK
[
  {
    "id": "м01",
    "name": "Field_м01",
    "size": 1867537.1178551097,
    "locations": {
      "center": [45.6962567581079, 41.3380610642585],
      "polygon": [
        [45.7074047669366, 41.3346809239899],
        [45.707543073278, 41.3414148034017],
        [45.6850638023809, 41.3414148034017],
        [45.6849600309502, 41.3347304378091],
        [45.7074047669366, 41.3346809239899]
      ]
    }
  },
  ...
]

2. Получить площадь поля

Запрос:
GET /api/fields/м01/size HTTP/1.1
Host: localhost:6347

Ответ 200 OK
1867537.1178551097

3. Расстояние от центра поля до точки

Запрос:
POST /api/fields/м01/distance HTTP/1.1
Host: localhost:6347
Content-Type: application/json
{
  "latitude": 45.7,
  "longitude": 41.34
}

Ответ 200 OK

1034.42
(в метрах, примерное значение)

4. Проверка принадлежности точки к полям

Запрос:
POST /api/fields/point-in-field HTTP/1.1
Host: localhost:6347
Content-Type: application/json
{
  "latitude": 45.7,
  "longitude": 41.34
}

ответ 200 OK
{
  "id": "м01",
  "name": "Field_м01"
}

Или если точка не попадает в поля
false
