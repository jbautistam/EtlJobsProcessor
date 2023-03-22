-- Elimina la tabla SalesGroupedByStore si existía
DROP TABLE IF EXISTS SalesGroupedByStore;

-- Inserta los datos agrupados por tienda en la tabla SalesGroupedByStore
CREATE TABLE SalesGroupedByStore AS 
	SELECT StoreId, SUM(Units) AS Units, SUM(Price) AS Price
		FROM Sales
		GROUP BY StoreId;