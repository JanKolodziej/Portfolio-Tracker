CREATE TABLE SP500Dolar AS
SELECT
    sub."Data" AS Data,
    sub."Kurs sredni" AS KursDolara,
    (sub."Close" + sub."High" + sub."Low") / 3.0 AS CenaSrednia
FROM (
    SELECT
        d."Data",
        d."Kurs sredni",
        sp."Date",
        sp."Close",
        sp."High",
        sp."Low",
        ROW_NUMBER() OVER (
            PARTITION BY d."Data"
            ORDER BY ABS(julianday(d."Data") - julianday(sp."Date"))
        ) AS rn
    FROM HistoriaSP sp
    CROSS JOIN HistoriaKursDolara d
) AS sub
WHERE rn = 1;