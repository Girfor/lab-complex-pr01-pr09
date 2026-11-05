# ПР3 — Data Mapping

Внешняя сущность `SupplyItem` преобразуется в `ReceiveReagentRequest`.

| Источник | Приемник | Правило |
|---|---|---|
| supplyId | deliveryId | Непустой код поставки |
| materialCode | externalCode | Непустой код вещества |
| title | name | Trim, не более 200 символов |
| lot | lot | Непустой номер партии |
| qty | quantity | decimal, точка как разделитель |
| uom | unit | kg → g ×1000, g → g, ml → ml |
| expiresOn | expirationDate | Формат yyyy-MM-dd |
| vendor | supplierCode | Код поставщика |

Идемпотентность обеспечивается парой supplyId + lot. При повторном приеме
возвращается существующая запись; при несовместимых ключевых данных — конфликт.
Нельзя конвертировать объем в массу без данных о плотности.

Код: `src/AIS.Infrastructure/SupplyMapper.cs`.
