with daily as
(
SELECT  CONVERT(varchar(10), DateTime, 120) AS date, 
		  MIN(Price) AS y1, 
		  SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * Amount) AS y2, 
		  SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * Amount * LOG (ABS(PriceOffset) * 100 + 1.1)) AS y3, 
		  COUNT(1) AS y4, 
		  SUM(PriceOffset * Amount) AS y5, 
		  SUM(Volume) / COUNT(1) AS y6,
		  --'open' AS y7,
		  MAX(PRICE) AS y8,
		  MIN(Price) AS y9, 
		  --'close' AS  y10,
		  SUM(Volume) AS y11,
		  SUM(Amount) AS y12,
		  SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 50000 THEN [Amount] ELSE 0 END) AS y13,
		  SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 200000 THEN [Amount] ELSE 0 END) AS y14,
		  SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 500000 THEN [Amount] ELSE 0 END) AS y15,
		  SUM(CASE WHEN [Direction] = 'B' THEN 1 WHEN [Direction] = 'S' THEN - 1 ELSE 0 END * CASE WHEN [Amount] > 1000000 THEN [Amount] ELSE 0 END) AS y16,
		  'sum y2' AS y17,
		  '(curent buy sum - per buy sum) - (curent sell sum - per sell sum) >0?buy:sell * Amount' AS y22,
		  code
    FROM     dbo.Detail d WITH (nolock)
    WHERE  code='sh600111' AND (DateTime >='2007-01-01' ) AND (DateTime < '2017-01-10')
    GROUP BY Code, CONVERT(varchar(10), DateTime, 120)
)
select d.*
,roi.openprice,roi.closeprice 
from daily d
join [RightsOfferingInfo] roi on d.code=roi.code and d.date=CONVERT(varchar(10), roi.datetime, 120)
