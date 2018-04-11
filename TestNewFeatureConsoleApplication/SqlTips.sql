SELECT COALESCE(ListPrice,0) ListPrice
FROM Production.Product
--------------------------------------------------------------------------------------------------
--OFFSET-FETCH
--------------------------------------------------------------------------------------------------
--<offset_fetch> ::= {OFFSET { integer_constant | offset_row_count_expression } { ROW | ROWS }    [FETCH { FIRST | NEXT } {integer_constant | fetch_row_count_expression } { ROW | ROWS } ONLY]}

SELECT *
FROM Production.Product
ORDER BY ListPrice DESC
OFFSET 0 ROWS FETCH FIRST 25 ROWS ONLY

SELECT *
FROM Production.Product
ORDER BY ListPrice DESC
OFFSET 10 ROWS FETCH NEXT 25 ROWS ONLY

--equal top 25
SELECT *
FROM Production.Product
ORDER BY ListPrice DESC
OFFSET 0 ROWS FETCH FIRST 25 ROWS ONLY


--The ORDER BY is actually mandatory when using OFFSET-FETCH
/*
SELECT *
FROM Production.Product
--ORDER BY ListPrice DESC
OFFSET 10 ROWS FETCH NEXT 25 ROWS ONLY

Msg 102, Level 15, State 1, Line 4
Incorrect syntax near '10'.
Msg 153, Level 15, State 2, Line 4
Invalid usage of the option NEXT in the FETCH statement.
*/

--ignore order by
SELECT *
FROM Production.Product
ORDER BY (SELECT NULL)
OFFSET 1 ROW FETCH FIRST 25 ROWS ONLY


--------------------------------------------------------------------------------------------------
--Where Like
--------------------------------------------------------------------------------------------------
/*
The following wildcards can be used in the LIKE operator:
%                = none, one or more characters
_                = a single character
[<character list>]        = a single character from the list
[<character range>]        = a single character from the range
[^<character list or range>]    = a single character that is not in the list or range
Here are some examples of all wildcards. Notice that the first and second example return the same results.
*/
-- First character is anything
-- Second character is an L
-- Third character is a space
SELECT *
FROM Production.Product
WHERE Name LIKE '_L %'
 
-- First character is an M, H or L
-- Second character is an L
SELECT *
FROM Production.Product
WHERE Name LIKE '[MHL]L%'
 
-- First character is an A, B or C
SELECT *
FROM Production.Product
WHERE Name LIKE '[A-C]%'
 
-- First character is not an A, B or C
SELECT *
FROM Production.Product
WHERE Name LIKE '[^A-C]%'

--------------------------------------------------------------------------------------------------
--ORDER BY 1-based index of column(bad practice)
--------------------------------------------------------------------------------------------------
--It is also possible to order by the (1-based) index of the column, 
--but this is considered bad practice. The following example shows this. 
--In this query the result is ordered by Name (because it is the first column in the SELECT list).
SELECT Name, ProductNumber
FROM Production.Product
ORDER BY 1



--------------------------------------------------------------------------------------------------
--OVER Function
--------------------------------------------------------------------------------------------------
--As you can see we can apply aggregating functions without grouping SalesOrderID and SalesOrderNumber. 
--Each row now simply shows the number of orders, the number of orders that have a sales person and 
--the average, maximum, minimum and total subtotal.

--That is pretty useful, but often you want to show these results per group. 
--For example you need to show the total subtotal for the customer in the row. 
--This can be done using PARTITION BY in the OVER clause.
--When looking at the result you can see the TotalSubTotal is still the same for every row, 
--but the TotalSubTotalPerCustomer is the same for each row with the same customer and different for each row with another customer.  
--Your result set, or window, is grouped by CustomerID (defined in PARTITION BY) and a single result is returned for each row.
SELECT
    SalesOrderID,
    SalesOrderNumber,
    COUNT(*) OVER()            AS NoOfOrders,
    COUNT(SalesPersonID) OVER()    AS OrdersWithSalesPerson,
    AVG(SubTotal) OVER()        AS AvgSubTotal,
    MAX(SubTotal) OVER()        AS MaxSubTotal,
    MIN(SubTotal) OVER()        AS MinSubTotal,
    SUM(SubTotal) OVER()        AS TotalSubTotal,
    SUM(SubTotal) OVER(PARTITION BY CustomerID) AS TotalSubTotalPerCustomer,
    SUM(SubTotal) OVER(PARTITION BY CustomerID, SalesPersonID) AS TotalSubTotalPerCustomerPerSalesPerson,

    /*
    ROWS BETWEEN UNBOUNDED PRECEDING part, which means sum all sub totals of the previous rows in the current group. 
    The last part AND CURRENT ROW means until the current row. An ORDER BY clause is mandatory 
    because without it the ROWS BETWEEN ... AND ... would have no meaning (since it would mean any random rows). 

    You can also switch the UNBOUNDED PRECEDING and CURRENT ROW around. 
    Doing this will result in the inverse of cumulating rows. 
    The first row will now have the sum of all rows for that customer, 
    while the second row will have the value of the first row minus the SubTotal of the first row.

    Notice that instead of <code>PRECEDING we need to use FOLLOWING because we now need to look at the next rows instead of the previous.
    We can also specify a number of rows that should be looked back to or looked ahead. 
    The following query gives the sum of the previous row and the current row (even if there are more rows preceding the current one). 
    */
    SUM(SubTotal) OVER(PARTITION BY CustomerID
                ORDER BY OrderDate
                ROWS BETWEEN UNBOUNDED PRECEDING
                AND CURRENT ROW) AS TotalSubTotalSoFarForCustomer,
    SUM(SubTotal) OVER(PARTITION BY CustomerID
                ORDER BY OrderDate
                ROWS BETWEEN CURRENT ROW
                AND UNBOUNDED FOLLOWING) AS InverseCumulative,
    SUM(SubTotal) OVER(PARTITION BY CustomerID
                ORDER BY OrderDate
                ROWS BETWEEN 1 PRECEDING
                AND CURRENT ROW) AS SumOfLastAndCurrentRowForCustomer,
    SUM(SubTotal) OVER() AS TotalSubTotal
FROM Sales.SalesOrderHeader  

--------------------------------------------------------------------------------------------------
--Ranking functions
--------------------------------------------------------------------------------------------------
/*
Ranking functions can rank rows in a window based on a specified ordering. 
There are four ranking functions within SQL Server; 
ROW_NUMBER, RANK, DENSE_RANK and NTILE. An ORDER BY clause is mandatory, 
a PARTITION is not (if it is not specified the entire window is considered one group). 

ROW_NUMBER speaks for itself. It simply returns the number of the current row.
RANK and DENSE_RANK assign a number to each row with a unique order value. 
That means that rows with the same order value (in this case CustomerID) get the same number. 
The difference between RANK and DENSE_RANK is that RANK assigns the current rank number 
plus the number of rows that have the same order value to the next row while 
DENSE_RANK always assigns the previous rank number plus one, no matter how many rows had the same order value.
NTILE partitions, or tiles, the rows in groups of equal size. 
In this case the returned result had 31465 rows and we requested 5000 tiles of equal size. 
31456 divided by 5000 equals 6 with a remainder of 1456. That means the NTILE value is increased after every six rows.
 Because there is a remainder of 1456 the first 1456 tiles get an additional row. 

 Notice that in this last example I have not added the SalesOrderID ordering to the RANK functions. 
 These return the same value for rows with the same CustomerID no matter their ordering within their group. 
 Adding an extra ordering column will actually change the meaning of the RANK functions!
*/

SELECT
    SalesOrderID,
    SalesOrderNumber,
    CustomerID,
    ROW_NUMBER() OVER(ORDER BY CustomerID) AS RowNumber,
    RANK() OVER(ORDER BY CustomerID) AS [Rank],
    DENSE_RANK() OVER(ORDER BY CustomerID) AS DenseRank,
    NTILE(5000) OVER(ORDER BY CustomerID) AS NTile5000
FROM Sales.SalesOrderHeader
ORDER BY CustomerID  

SELECT
    SalesOrderID,
    SalesOrderNumber,
    CustomerID,
    ROW_NUMBER() OVER(ORDER BY CustomerID, SalesOrderID) AS RowNumber,
    RANK() OVER(ORDER BY CustomerID) AS [Rank],
    DENSE_RANK() OVER(ORDER BY CustomerID) AS DenseRank,
    NTILE(5000) OVER(ORDER BY CustomerID, SalesOrderID) AS NTile5000
FROM Sales.SalesOrderHeader
ORDER BY CustomerID

--------------------------------------------------------------------------------------------------
--Offset functions
--------------------------------------------------------------------------------------------------
/*
Offset functions can return a value from the first or last row in a window or from a row a specified rows away from the current row. 
There are four offset functions LAG, LEAD, FIRST_VALUE and LAST_VALUE.

LAG and LEAD work in much the same way, except LAG looks at rows that come before the current row while LEAD looks at rows that come after the current row. 
The syntax is actually much like that of an aggregate function. 

*/
SELECT
    SalesOrderID,
    SalesOrderNumber,
    LAG(CustomerID) OVER(ORDER BY OrderDate) AS PreviousCustomer,
    CustomerID AS CurrentCustomer,
    LEAD(CustomerID) OVER(ORDER BY OrderDate) AS NextCustomer
FROM Sales.SalesOrderHeader
ORDER BY OrderDate

/*
We can pass a second parameter to the LAG and LEAD functions specifying how many rows we should skip. 
The following query shows us a bit about the frequency of orders per customer. 
We show the shipdate of the order before the last two orders of the customer (if there is any) 
and also the shipdate of the order after the next order of the customer (if there is any).
*/
SELECT
    SalesOrderID,
    SalesOrderNumber,
    CustomerID,
    OrderDate,
    ShipDate,
    LAG(ShipDate, 3) OVER(PARTITION BY CustomerID
                ORDER BY ShipDate) AS ShipDateThreeOrdersAgo,
    LEAD(ShipDate, 2) OVER(PARTITION BY CustomerID
                ORDER BY ShipDate) AS ShipDateOfOrderAfterNext
FROM Sales.SalesOrderHeader
ORDER BY OrderDate

/*
We can also do something about those NULLs when the specified row was not found. 
The LAG and LEAD functions can take a third parameter that specifies a placeholder for NULLs. 
Make sure you specify a value of the same type as your column or you will get an error unless you make a CAST (which is discussed in the next part of the article). 
The following query shows the previous or next sales order number for each customer.
When there is no previous or next order the text "No previous/next orders" is displayed (instead of NULL).
*/
SELECT
    SalesOrderID,
    CustomerID,
    LAG(SalesOrderNumber, 1, 'No previous orders')
        OVER(PARTITION BY CustomerID
        ORDER BY OrderDate) AS PreviousOrderForCustomer,
    SalesOrderNumber AS CurrentOrderNumber,
    LEAD(SalesOrderNumber, 1, 'No next orders')
        OVER(PARTITION BY CustomerID
        ORDER BY OrderDate) AS NextOrderForCustomer
FROM Sales.SalesOrderHeader
ORDER BY OrderDate


SELECT
    SalesOrderID,
    CustomerID,
    SalesOrderNumber AS CurrentOrder,
    FIRST_VALUE(SalesOrderNumber) OVER(ORDER BY SalesOrderNumber) AS FirstOrder,
    LAST_VALUE(SalesOrderNumber) OVER(ORDER BY SalesOrderNumber) AS LastOrder
FROM Sales.SalesOrderHeader
ORDER BY OrderDate 

/*
As you can see in the result each row now shows the first and the last order number. 
There is something fishy about the last order number though, it always has the value of the current row, 
as if that is the last row that is evaluated.  That is actually true. 
Just like aggregate functions <code>FIRST_VALUE and LAST_VALUE support framing.
 So if we wanted to show the last order ever we would have to explicitly indicate this by framing. 
 As you can see the following query does return the expected result.
*/
SELECT
    SalesOrderID,
    CustomerID,
    SalesOrderNumber AS CurrentOrder,
    FIRST_VALUE(SalesOrderNumber)
        OVER(ORDER BY SalesOrderNumber
            ROWS BETWEEN UNBOUNDED PRECEDING
            AND CURRENT ROW) AS FirstOrder,
    LAST_VALUE(SalesOrderNumber)
        OVER(ORDER BY SalesOrderNumber
            ROWS BETWEEN CURRENT ROW
            AND UNBOUNDED FOLLOWING) AS LastOrder
FROM Sales.SalesOrderHeader
ORDER BY OrderDate

--Of course we can also use partitions to get the first and last order per customer.
SELECT
    SalesOrderID,
    CustomerID,
    SalesOrderNumber AS CurrentOrder,
    FIRST_VALUE(SalesOrderNumber)
        OVER(PARTITION BY CustomerID
            ORDER BY SalesOrderNumber
            ROWS BETWEEN UNBOUNDED PRECEDING
            AND CURRENT ROW) AS FirstOrder,
    LAST_VALUE(SalesOrderNumber)
        OVER(PARTITION BY CustomerID
            ORDER BY SalesOrderNumber
            ROWS BETWEEN CURRENT ROW
            AND UNBOUNDED FOLLOWING) AS LastOrder
FROM Sales.SalesOrderHeader
ORDER BY OrderDate

--------------------------------------------------------------------------------------------------
--More filtering options; IN, ANY, SOME, ALL and EXISTS
--------------------------------------------------------------------------------------------------
SELECT *
FROM Person.Person AS p
WHERE NOT p.BusinessEntityID IN (SELECT PersonID
                FROM Sales.Customer
                WHERE PersonID IS NOT NULL) 

DECLARE @OrderDate AS DATETIME = '20050517'
DECLARE @Status AS TINYINT = 4
IF @Status > ANY(SELECT Status
        FROM Purchasing.PurchaseOrderHeader
        WHERE OrderDate = @OrderDate)
    PRINT 'Not all orders have the specified status!'
ELSE
    PRINT 'All orders have the specified status.' 

DECLARE @OrderDate AS DATETIME = '20050517'
DECLARE @Status AS TINYINT = 4
IF @Status > SOME(SELECT Status
        FROM Purchasing.PurchaseOrderHeader
        WHERE OrderDate = @OrderDate)
    PRINT 'Not all orders have the specified status!'
ELSE
    PRINT 'All orders have the specified status.'


DECLARE @OrderDate AS DATETIME = '20050517'
DECLARE @Status AS TINYINT = 4
IF @Status < ALL(SELECT Status
        FROM Purchasing.PurchaseOrderHeader
        WHERE OrderDate = @OrderDate)
    PRINT 'All orders have the specified status.'
ELSE
    PRINT 'Not all orders have the specified status!'

SELECT *
FROM Sales.Customer AS c
WHERE NOT EXISTS(SELECT *
        FROM Sales.SalesOrderHeader AS s
        WHERE s.CustomerID = c.CustomerID)
        
--------------------------------------------------------------------------------------------------
--Recursion with CTEs and UNION ALL, INTERSECT, EXCEPT 
--------------------------------------------------------------------------------------------------
/*
    
*/
WITH REC AS
(
    SELECT
        BusinessEntityID,
        FirstName,
        LastName
    FROM Person.Person
    WHERE BusinessEntityID = 9
 
    UNION ALL
 
    SELECT
        p.BusinessEntityID,
        p.FirstName,
        p.LastName
    FROM REC
        JOIN Person.Person AS p ON p.BusinessEntityID = REC.BusinessEntityID - 1
)
SELECT *
FROM REC;
    
WITH REC AS (
    SELECT 101 AS SomeCounter
    
    UNION ALL

    SELECT SomeCounter - 1
    FROM REC
    WHERE SomeCounter - 1 >= 0
)
SELECT *
FROM REC
OPTION (MAXRECURSION 200)     

/*
INTERSECT is another set operator and the syntax and rules are the same as that of the UNION operator. 
The difference between the two is the results that are returned. 
UNION returns all rows and discards duplicate rows. INTERSECT returns only duplicate rows (once). 
Let us take the first example I used for UNION, but replace the UNION with an INTERSECT.
*/
SELECT TOP 1
    BusinessEntityID,
    Title,
    FirstName,
    MiddleName,
    LastName
FROM Person.Person
INTERSECT
SELECT TOP 2
    BusinessEntityID,
    Title,
    FirstName,
    MiddleName,
    LastName
FROM Person.Person
ORDER BY BusinessEntityID 


/*
EXCEPT is the third set operator and the syntax and rules are also the same as for that of the UNION operator. 
EXCEPT returns only the records from the first query that are not returned by the second query. In other words, 
EXCEPT returns rows that are unique to the first query. We should notice here that with UNION and INTERSECT
it does not matter which query comes first and which query comes second, the result remains the same. 
With EXCEPT the order of queries does matter. 
I will show this by using the same example I used for UNION and INTERSECT, but use EXCEPT instead. 

*/
SELECT TOP 2
    BusinessEntityID,
    Title,
    FirstName,
    MiddleName,
    LastName
FROM Person.Person
EXCEPT
SELECT TOP 1
    BusinessEntityID,
    Title,
    FirstName,
    MiddleName,
    LastName
FROM Person.Person
ORDER BY BusinessEntityID
--------------------------------------------------------------------------------------------------
--PARSE (Transact-SQL)
--TRY_PARSE
--------------------------------------------------------------------------------------------------
go
/*
PARSE ( string_value AS data_type [ USING culture ] )
Category		Type			.NET Framework type		Styles used
Numeric			bigint				Int64				NumberStyles.Number
Numeric			int					Int32				NumberStyles.Number
Numeric			smallint			Int16				NumberStyles.Number
Numeric			tinyint				Byte				NumberStyles.Number
Numeric			decimal				Decimal				NumberStyles.Number
Numeric			numeric				Decimal				NumberStyles.Number
Numeric			float				Double				NumberStyles.Float
Numeric			real				Single				NumberStyles.Float
Numeric			smallmoney			Decimal				NumberStyles.Currency
Numeric			money				Decimal				NumberStyles.Currency
Date and Time	date				DateTime			DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal
Date and Time	time				TimeSpan			DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal
Date and Time	datetime			DateTime			DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal
Date and Time	smalldatetime		DateTime			DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal
Date and Time	datetime2			DateTime			DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal
Date and Time	datetimeoffset		DateTimeOffset		DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal


Full name					Alias					LCID			Specific culture
us_english					English					1033			en-US
Deutsch						German					1031			de-DE
Français					French					1036			fr-FR
日本語						Japanese				1041			ja-JP
Dansk						Danish					1030			da-DK
Español						Spanish					3082			es-ES
Italiano					Italian					1040			it-IT
Nederlands					Dutch					1043			nl-NL
Norsk						Norwegian				2068			nn-NO
Português					Portuguese				2070			pt-PT
Suomi						Finnish					1035			fi	
Svenska						Swedish					1053			sv-SE
čeština						Czech					1029			Cs-CZ
magyar						Hungarian				1038			Hu-HU
polski						Polish					1045			Pl-PL
română						Romanian				1048			Ro-RO
hrvatski					Croatian				1050			hr-HR
slovenčina					Slovak					1051			Sk-SK
slovenski					Slovenian				1060			Sl-SI
ελληνικά					Greek					1032			El-GR
български					Bulgarian				1026			bg-BG
русский						Russian					1049			Ru-RU
Türkçe						Turkish					1055			Tr-TR
British						British English			2057			en-GB
eesti						Estonian				1061			Et-EE
latviešu					Latvian					1062			lv-LV
lietuvių					Lithuanian				1063			lt-LT
Português (Brasil)			Brazilian				1046			pt-BR
繁體中文						Traditional Chinese		1028			zh-TW
한국어						Korean					1042			Ko-KR
简体中文						Simplified Chinese		2052			zh-CN
Arabic						Arabic					1025			ar-SA
ไทย							Thai					1054			Th-TH
*/
SELECT PARSE('Monday, 13 December 2010' AS datetime2 USING 'en-US') AS Result;
SELECT PARSE('2010' AS int) AS Result;

-- The English language is mapped to en-US specific culture
SET LANGUAGE 'English';
SELECT PARSE('12/16/2010' AS datetime2) AS Result;

SELECT TRY_PARSE('Jabberwokkie' AS datetime2 USING 'en-US') AS Result;

SELECT
    CASE WHEN TRY_PARSE('Aragorn' AS decimal USING 'sr-Latn-CS') IS NULL
        THEN 'True'
        ELSE 'False'
END
AS Result;

SET LANGUAGE English;
SELECT IIF(TRY_PARSE('01/01/2011' AS datetime2) IS NULL, 'True', 'False') AS Result;

--------------------------------------------------------------------------------------------------
--TRY_CONVERT
--------------------------------------------------------------------------------------------------
--TRY_CONVERT returns null
--The following example demonstrates that TRY_CONVERT returns null when the cast fails.
SELECT 
    CASE WHEN TRY_CONVERT(float, 'test') IS NULL 
    THEN 'Cast failed'
    ELSE 'Cast succeeded'
END AS Result;
GO

--The following example demonstrates that the expression must be in the expected format.
SET DATEFORMAT dmy;
SELECT TRY_CONVERT(datetime2, '12/31/2010') AS Result;--return null
GO

--TRY_CONVERT fails with an error
--The following example demonstrates that TRY_CONVERT returns an error when the cast is explicitly not permitted.
SELECT TRY_CONVERT(xml, 4) AS Result;
GO
--Msg 529, Level 16, State 2, Line 1
--Explicit conversion from data type int to xml is not allowed.

--TRY_CONVERT succeeds
--This example demonstrates that the expression must be in the expected format.
SET DATEFORMAT mdy;
SELECT TRY_CONVERT(datetime2, '12/31/2010') AS Result;
GO

--------------------------------------------------------------------------------------------------
--DATEFROMPARTS
--EOMONTH
--CHOOSE 
--IIF 
--CONCAT
--REPLICATE 
--SOUNDEX
--DIFFERENCE
--QUOTENAME
--STUFF 
--------------------------------------------------------------------------------------------------

SELECT DATEFROMPARTS ( 2010, 12, 31 ) AS Result;--DATEFROMPARTS ( year, month, day )

DECLARE @date DATETIME = '15/1/2011';
SELECT EOMONTH ( @date ) AS Result; --Returns the last day of the month that contains the specified date, with an optional offset.

SELECT CHOOSE ( 3, 'Manager', 'Director', 'Developer', 'Tester' ) AS Result; --Developer Returns the item at the specified index from a list of values in SQL Server 2012.

SELECT JobTitle, HireDate, 
CHOOSE(MONTH(HireDate),'Winter','Winter', 'Spring','Spring','Spring','Summer','Summer', 
                       'Summer','Autumn','Autumn','Autumn','Winter') AS Quarter_Hired
FROM HumanResources.Employee
WHERE  YEAR(HireDate) > 2005
ORDER BY YEAR(HireDate)

--IIF ( boolean_expression, true_value, false_value )
DECLARE @a int = 45, @b int = 40;
SELECT IIF ( @a > @b, 'TRUE', 'FALSE' ) AS Result;

SELECT CONCAT ( 'Happy ', 'Birthday ', 11, '/', '25' ) AS Result; --Happy Birthday 11/25   Returns a string that is the result of concatenating two or more string values.

--Repeats a string value a specified number of times.
--REPLICATE ( string_expression ,integer_expression ) 
SELECT REPLICATE('0',5),REPLICATE('100',3) --00000	100100100

--Returns a four-character (SOUNDEX) code to evaluate the similarity of two strings.
--SOUNDEX ( character_expression )
/*SOUNDEX converts an alphanumeric string to a four-character code that is based on how the string sounds when spoken. The first character of the code is the first character of character_expression, converted to upper case. The second through fourth characters of the code are numbers that represent the letters in the expression. The letters A, E, I, O, U, H, W, and Y are ignored unless they are the first letter of the string. Zeroes are added at the end if necessary to produce a four-character code. For more information about the SOUNDEX code, see The Soundex Indexing System.
SOUNDEX codes from different strings can be compared to see how similar the strings sound when spoken. The DIFFERENCE function performs a SOUNDEX on two strings, and returns an integer that represents how similar the SOUNDEX codes are for those strings.
SOUNDEX is collation sensitive. String functions can be nested.
*/
-- Using SOUNDEX
SELECT SOUNDEX ('Smith'), SOUNDEX ('Smythe'); --S530  S530  Here is the result set. Valid for a Latin1_General collation.

--DIFFERENCE ( character_expression , character_expression )
--Returns an integer value that indicates the difference between the SOUNDEX values of two character expressions.
--The DIFFERENCE function compares the difference of the SOUNDEX pattern results. The following example shows two strings that differ only in vowels. The difference returned is 4, the lowest possible difference.
-- Using DIFFERENCE
SELECT DIFFERENCE('Smithers', 'Smythers');
GO

--STUFF ( character_expression , start , length , replaceWith_expression )
--The STUFF function inserts a string into another string. It deletes a specified length of characters in the first string at the start position and then inserts the second string into the first string at the start position.
--The following example returns a character string created by deleting three characters from the first string, abcdef, starting at position 2, at b, and inserting the second string at the deletion point.
SELECT STUFF('aaaxxxeee', 4, 3, 'bbbcccddd'); --aaabbbcccdddeee


--Returns a Unicode string with the delimiters added to make the input string a valid SQL Server delimited identifier.
--QUOTENAME ( 'character_string' [ , 'quote_character' ] ) 
--The following example takes the character string abc[]def and uses the [ and ] characters to create a valid SQL Server delimited identifier.
SELECT QUOTENAME('abc[]def') --[abc[]]def]

--There are other types of qualifiers supported such as tags, brackets, braces and parenthesis.  It appears others make the function return null.
DECLARE @string VARCHAR(30)= 'Some String'
SELECT 'Tag' AS [Type], QUOTENAME(@string, '<') [QuotedValue]
UNION ALL
SELECT 'Parentheses', QUOTENAME(@string, '(')
UNION ALL
SELECT 'Brace', QUOTENAME(@string, '{')
UNION ALL
SELECT 'Bracket', QUOTENAME(@string, '[')
UNION ALL
SELECT 'Tick', QUOTENAME(@string, '''')
UNION ALL
SELECT 'Dot', QUOTENAME(@string, '.')


--------------------------------------------------------------------------------------------------
--FORMAT 
--------------------------------------------------------------------------------------------------
--FORMAT ( value, format [, culture ] )
/*
Category				Type				.NET type
Numeric					bigint				Int64
Numeric					int					Int32
Numeric					smallint			Int16
Numeric					tinyint				Byte
Numeric					decimal				SqlDecimal
Numeric					numeric				SqlDecimal
Numeric					float				Double
Numeric					real				Single
Numeric					smallmoney			Decimal
Numeric					money				Decimal
Date and Time			date				DateTime
Date and Time			time				TimeSpan
Date and Time			datetime			DateTime
Date and Time			smalldatetime		DateTime
Date and Time			datetime2			DateTime
Date and Time			datetimeoffset		DateTimeOffset
*/
DECLARE @d DATETIME = '10/01/2011';
SELECT FORMAT ( @d, 'd', 'en-US' ) AS 'US English Result'
      ,FORMAT ( @d, 'd', 'en-gb' ) AS 'Great Britain English Result'
      ,FORMAT ( @d, 'd', 'de-de' ) AS 'German Result'
      ,FORMAT ( @d, 'd', 'zh-cn' ) AS 'Simplified Chinese (PRC) Result'; 
  
SELECT FORMAT ( @d, 'D', 'en-US' ) AS 'US English Result'
      ,FORMAT ( @d, 'D', 'en-gb' ) AS 'Great Britain English Result'
      ,FORMAT ( @d, 'D', 'de-de' ) AS 'German Result'
      ,FORMAT ( @d, 'D', 'zh-cn' ) AS 'Chinese (Simplified PRC) Result'; 
/*
US English Result Great Britain English Result  German Result Simplified Chinese (PRC) Result
----------------  ----------------------------- ------------- -------------------------------------
10/1/2011         01/10/2011                    01.10.2011    2011/10/1

(1 row(s) affected)

US English Result            Great Britain English Result  German Result                    Chinese (Simplified PRC) Result
---------------------------- ----------------------------- -----------------------------  ---------------------------------------
Saturday, October 01, 2011   01 October 2011               Samstag, 1. Oktober 2011        2011年10月1日

(1 row(s) affected)
*/

-- Current date is September 27 2012.
DECLARE @d DATETIME = GETDATE();
SELECT FORMAT( @d, 'dd/MM/yyyy', 'en-US' ) AS 'DateTime Result'
       ,FORMAT(123456789,'###-##-####') AS 'Custom Number Result';

/*
DateTime Result  Custom Number Result
--------------   --------------------
27/09/2012       123-45-6789
(1 row(s) affected)
*/
SELECT TOP(5)CurrencyRateID, EndOfDayRate
            ,FORMAT(EndOfDayRate, 'N', 'en-us') AS 'Number Format'
            ,FORMAT(EndOfDayRate, 'G', 'en-us') AS 'General Format'
            ,FORMAT(EndOfDayRate, 'C', 'en-us') AS 'Currency Format'
FROM Sales.CurrencyRate
ORDER BY CurrencyRateID;
/*
CurrencyRateID EndOfDayRate  Numeric Format  General Format  Currency Format
-------------- ------------  --------------  --------------  ---------------
1              1.0002        1.00            1.0002          $1.00
2              1.55          1.55            1.5500          $1.55
3              1.9419        1.94            1.9419          $1.94
4              1.4683        1.47            1.4683          $1.47
5              8.2784        8.28            8.2784          $8.28
(5 row(s) affected)
*/

SELECT TOP(5)CurrencyRateID, EndOfDayRate
            ,FORMAT(EndOfDayRate, 'N', 'de-de') AS 'Numeric Format'
,FORMAT(EndOfDayRate, 'G', 'de-de') AS 'General Format'
,FORMAT(EndOfDayRate, 'C', 'de-de') AS 'Currency Format'
FROM Sales.CurrencyRate
ORDER BY CurrencyRateID;
/*CurrencyRateID EndOfDayRate  Numeric Format  General Format  Currency Format
-------------- ------------  --------------  --------------  ---------------
1              1.0002        1,00            1,0002          1,00 €
2              1.55          1,55            1,5500          1,55 €
3              1.9419        1,94            1,9419          1,94 €
4              1.4683        1,47            1,4683          1,47 €
5              8.2784        8,28            8,2784          8,28 €
 (5 row(s) affected)
*/
go
--------------------------------------------------------------------------------------------------
--TERTIARY_WEIGHTS 
--COLLATIONPROPERTY
--fn_helpcollations
--COLLATE
--https://msdn.microsoft.com/en-us/library/ms143726(v=sql.110).aspx
--------------------------------------------------------------------------------------------------
go
/*
Sort order ID			SQL collation
33						SQL_Latin1_General_Pref_CP437_CI_AS
34						SQL_Latin1_General_CP437_CI_AI
43						SQL_Latin1_General_Pref_CP850_CI_AS
44						SQL_Latin1_General_CP850_CI_AI
49						SQL_1xCompat_CP850_CI_AS
53						SQL_Latin1_General_Pref_CP1_CI_AS
54						SQL_Latin1_General_CP1_CI_AI
56						SQL_AltDiction_Pref_CP850_CI_AS
57						SQL_AltDiction_CP850_CI_AI
58						SQL_Scandinavian_Pref_CP850_CI_AS
82						SQL_Latin1_General_CP1250_CI_AS
84						SQL_Czech_CP1250_CI_AS
86						SQL_Hungarian_CP1250_CI_AS
88						SQL_Polish_CP1250_CI_AS
90						SQL_Romanian_CP1250_CI_AS
92						SQL_Croatian_CP1250_CI_AS
94						SQL_Slovak_CP1250_CI_AS
96						SQL_Slovenian_CP1250_CI_AS
106						SQL_Latin1_General_CP1251_CI_AS
108						SQL_Ukrainian_CP1251_CI_AS
113						SQL_Latin1_General_CP1253_CS_AS
114						SQL_Latin1_General_CP1253_CI_AS
130						SQL_Latin1_General_CP1254_CI_AS
146						SQL_Latin1_General_CP1256_CI_AS
154						SQL_Latin1_General_CP1257_CI_AS
156						SQL_Estonian_CP1257_CI_AS
158						SQL_Latvian_CP1257_CI_AS
160						SQL_Lithuanian_CP1257_CI_AS
183						SQL_Danish_Pref_CP1_CI_AS
184						SQL_SwedishPhone_Pref_CP1_CI_AS
185						SQL_SwedishStd_Pref_CP1_CI_AS
186						SQL_Icelandic_Pref_CP1_CI_AS
*/
go
--Returns a binary string of weights for each character in a non-Unicode string expression defined with an SQL tertiary collation.
--TERTIARY_WEIGHTS( non_Unicode_character_string_expression )
SELECT  TERTIARY_WEIGHTS([PasswordHash]) FROM [Person].[Password]

--The following example creates a computed column in a table that applies the TERTIARY_WEIGHTS function to the values of a char column.
CREATE TABLE TertColTable
(Col1 char(15) COLLATE SQL_Latin1_General_Pref_CP437_CI_AS,
Col2 AS TERTIARY_WEIGHTS(Col1));
GO 

--COLLATIONPROPERTY( collation_name , property )
/*
Property name	Description
CodePage		Non-Unicode code page of the collation.
LCID			Windows LCID of the collation. 
ComparisonStyle Windows comparison style of the collation. Returns 0 for all binary collations .
Version			The version of the collation, derived from the version field of the collation ID. Returns 2, 1, or 0.
                Collations with "100" in the name) return 2. Collations with "90" in the name) return 1. All other collations return 0.
*/
SELECT COLLATIONPROPERTY('Traditional_Spanish_CS_AS_KS_WS', 'CodePage') --1252

--Returns a list of all the collations supported by SQL Server 2012.
--SQL Server supports Windows collations. SQL Server also supports a limited number (<80) of collations called SQL Server collations which were developed before SQL Server supported Windows collations. SQL Server collations are still supported for backward compatibility, but should not be used for new development work. For more information about Windows collations, see Windows Collation Name (Transact-SQL). For more information about collations, see Collation and Unicode Support.
SELECT Name, Description FROM fn_helpcollations()
WHERE Name like 'L%' AND Description LIKE '% binary sort';

SELECT firstname FROM person.person ORDER BY firstname COLLATE Latin1_General_CS_AI;

--------------------------------------------------------------------------------------------------
--PERCENTILE_DISC 
--PERCENTILE_CONT 
--PERCENT_RANK
--CUME_DIST
--https://msdn.microsoft.com/en-us/library/ms143726(v=sql.110).aspx
--------------------------------------------------------------------------------------------------
go
--CUME_DIST( ) OVER ( [ partition_by_clause ] order_by_clause )
--Calculates the cumulative distribution of a value in a group of values in SQL Server 2012. That is, CUME_DIST computes the relative position of a specified value in a group of values. For a row r, assuming ascending ordering, the CUME_DIST of r is the number of rows with values lower than or equal to the value of r, divided by the number of rows evaluated in the partition or query result set. CUME_DIST is similar to the PERCENT_RANK function.

--PERCENTILE_DISC ( numeric_literal ) WITHIN GROUP ( ORDER BY order_by_expression [ ASC | DESC ] ) OVER ( [ <partition_by_clause> ] )
--Computes a specific percentile for sorted values in an entire rowset or within distinct partitions of a rowset in SQL Server 2012. For a given percentile value P, PERCENTILE_DISC sorts the values of the expression in the ORDER BY clause and returns the value with the smallest CUME_DIST value (with respect to the same sort specification) that is greater than or equal to P. For example, PERCENTILE_DISC (0.5) will compute the 50th percentile (that is, the median) of an expression. PERCENTILE_DISC calculates the percentile based on a discrete distribution of the column values; the result is equal to a specific value in the column.

--PERCENTILE_CONT ( numeric_literal ) WITHIN GROUP ( ORDER BY order_by_expression [ ASC | DESC ] ) OVER ( [ <partition_by_clause> ] )
--Calculates a percentile based on a continuous distribution of the column value in SQL Server 2012. The result is interpolated and might not be equal to any of the specific values in the column. 

--PERCENT_RANK( ) OVER ( [ partition_by_clause ] order_by_clause )
--Calculates the relative rank of a row within a group of rows in SQL Server 2012. Use PERCENT_RANK to evaluate the relative standing of a value within a query result set or partition. PERCENT_RANK is similar to the CUME_DIST function. 


SELECT DISTINCT TOP 20
    Name AS DepartmentName
    ,ph.Rate
    ,CUME_DIST() OVER (PARTITION BY Name ORDER BY ph.Rate) AS CumeDist --current row number/total number, COMPUTE WITH TOTAL ROWS THAT NO DISTINCT
    ,PERCENT_RANK() OVER (PARTITION BY Name ORDER BY ph.Rate ) AS PctRank --(current row number-1)/(total number-1), COMPUTE WITH TOTAL ROWS THAT NO DISTINCT
    ,PERCENTILE_DISC(0.5) WITHIN GROUP (ORDER BY ph.Rate) OVER (PARTITION BY Name) AS MedianDisc --The first value greater or equal to CumeDist column
    ,PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY ph.Rate) OVER (PARTITION BY Name) AS MedianCont
    /*
        RN = (1 + (P * ( N - 1 ))
        P = Percentile Specified
        N = Number of rows
 
        CRN = CEILING(RN) 
        FRN = FLOOR(RN).
        If (CRN = FRN = RN) then the result is [value of expression from row at RN]
        Otherwise the result is
            (CRN - RN) * (value of expression for row at FRN) +
            (RN - FRN) * (value of expression for row at CRN)
    */
FROM HumanResources.Department AS d
INNER JOIN HumanResources.EmployeeDepartmentHistory AS dh 
    ON dh.DepartmentID = d.DepartmentID
INNER JOIN HumanResources.EmployeePayHistory AS ph
    ON ph.BusinessEntityID = dh.BusinessEntityID
WHERE dh.EndDate IS NULL;

/*
DepartmentName						Rate		CumeDist			PctRank				MedianDisc		MedianCont
Document Control					10.25		0.4					0					16.8269			16.8269
Document Control					16.8269		0.8					0.5					16.8269			16.8269
Document Control					17.7885		1					1					16.8269			16.8269
Engineering							32.6923		0.5					0					32.6923			34.375
Engineering							36.0577		0.666666667			0.6					32.6923			34.375
Engineering							43.2692		0.833333333			0.8					32.6923			34.375
Engineering							63.4615		1					1					32.6923			34.375
Executive							39.06		0.25				0					48.5577			54.32695
Executive							48.5577		0.5					0.333333333			48.5577			54.32695
Executive							60.0962		0.75				0.666666667			48.5577			54.32695
Executive							125.5		1					1					48.5577			54.32695
Facilities and Maintenance			9.25		0.571428571			0					9.25			9.25
Facilities and Maintenance			9.75		0.714285714			0.666666667			9.25			9.25
Facilities and Maintenance			20.4327		0.857142857			0.833333333			9.25			9.25
Facilities and Maintenance			24.0385		1					1					9.25			9.25
Finance								13.4615		0.1					0					19				19
Finance								19			0.6					0.111111111			19				19
Finance								26.4423		0.8					0.666666667			19				19
Finance								34.7356		0.9					0.888888889			19				19
Finance								43.2692		1					1					19				19

*/
go
--------------------------------------------------------------------------------------------------
--PIVOT
--------------------------------------------------------------------------------------------------

DECLARE @sql NVARCHAR(4000);
SELECT @sql=isnull(@sql+',','')+' SUM(CASE [OrganizationLevel] WHEN '''+ CAST(OrganizationLevel AS NVARCHAR) +''' THEN 1 ELSE 0 END) ['+ CAST(OrganizationLevel AS NVARCHAR) + ']'
FROM(select DISTINCT  OrganizationLevel FROM [HumanResources].[Employee]) AS a      
SET @sql='select [JobTitle],'+@sql+' from [HumanResources].[Employee] group by [JobTitle]'
--select @sql
EXEC(@sql)

--equal to 

SELECT [jobtitle], SUM([0]) [0], SUM([1]) [1], SUM([2]) [2], SUM([3]) [3], SUM([4]) [4]
FROM   
(
    SELECT [jobtitle], [0], [1], [2], [3], [4]
    FROM   [HumanResources].[Employee] PIVOT(COUNT([OrganizationLevel]) 
    FOR [OrganizationLevel] IN([0], [1], [2], [3], [4])) AS [piv]
) AS [rt]
GROUP BY [jobtitle], [0], [1], [2], [3], [4]
ORDER BY [jobtitle]
go

--equal to 
DECLARE @sql VARCHAR(8000)
DECLARE @columns VARCHAR(8000)
DECLARE @sum VARCHAR(8000)

SET @sql = ''
SET @columns = ''
SET @sum = ''
SELECT @columns = @columns+',['+CAST([OrganizationLevel] AS NVARCHAR)+']', @sum = @sum+',SUM(['+CAST([OrganizationLevel] AS NVARCHAR)+']) ['+CAST([OrganizationLevel] AS NVARCHAR)+']'
FROM   [HumanResources].[Employee]
GROUP BY [OrganizationLevel]
SET @columns = STUFF(@columns, 1, 1, '')
SET @sum = STUFF(@sum, 1, 1, '')
SET @sql =
'
SELECT [jobtitle], ' + @sum + '
FROM   
(
    SELECT [jobtitle], ' + @columns + '
    FROM   [HumanResources].[Employee] PIVOT(COUNT([OrganizationLevel]) 
    FOR [OrganizationLevel] IN(' + @columns + ')) AS [piv]
) AS [rt]
GROUP BY [jobtitle], ' + @columns + '
ORDER BY [jobtitle]
'
--SELECT @sql
EXEC (@sql) --The () is required and will got "is not a valid identifier" error when missing it
go

/*
create table tb(name nvarchar(10),course nvarchar(10),score int)

insert into tb values('张三','语文',74)

insert into tb values('张三','数学',83)

insert into tb values('张三','物理',93)

insert into tb values('李四','语文',74)

insert into tb values('李四','数学',84)

insert into tb values('李四','物理',94)

declare @sql nvarchar(4000);

select @sql=isnull(@sql+',','')+' max(case course when '''+course+''' then score else 0 end) ['+course+']'

from(select distinct course from tb)as a      

set @sql='select name,'+@sql+' from tb group by name'
select @sql
exec(@sql)
*/
go
--------------------------------------------------------------------------------------------------
--ROWGUIDCOL 
--IDENTITY
--------------------------------------------------------------------------------------------------
go
/*
Specifies that the column is a row globally unique identifier column. ROWGUIDCOL can only be assigned to a uniqueidentifier column, and only one uniqueidentifier column per table can be designated as the ROWGUIDCOL column. ROWGUIDCOL cannot be assigned to columns of user-defined data types.
ROWGUIDCOL does not enforce uniqueness of the values stored in the column. Also, ROWGUIDCOL does not automatically generate values for new rows that are inserted into the table. To generate unique values for each column, either use the NEWID function on INSERT statements or specify the NEWID function as the default for the column. For more information, see NEWID (Transact-SQL)and INSERT (Transact-SQL).
*/
CREATE TABLE dbo.TestTable
    (
    Id int NOT NULL IDENTITY (1, 1),
    Name nvarchar(50) NOT NULL,
    rowguid uniqueidentifier NOT NULL ROWGUIDCOL DEFAULT newsequentialid()
    )
GO

SELECT $ROWGUID,$IDENTITY FROM dbo.TestTable

--<column_definition> ::= column_name <data_type>  [ NULL | NOT NULL ] IDENTITY [ ( seed ,increment ) ] [ NOT FOR REPLICATION ] 
--SET IDENTITY_INSERT schema_name.table_name ON | OFF
--When a new row is added to the table, the Database Engine provides a unique, incremental value for the column. Identity columns are typically used with PRIMARY KEY constraints to serve as the unique row identifier for the table.
--    seed  Is the value used for the very first row loaded into the table.
--    increment   Is the incremental value added to the identity value of the previous row loaded.
--    NOT FOR REPLICATION  In the CREATE TABLE statement, the NOT FOR REPLICATION clause can be specified for the IDENTITY property, FOREIGN KEY constraints, and 

go
--------------------------------------------------------------------------------------------------
--hierarchyid 
--------------------------------------------------------------------------------------------------
--create demo table 
WITH CTE As (
 SELECT  [BusinessEntityID]
   ,[LoginID]
   ,[OrganizationNode]
   ,[OrganizationNode].ToString() As LogicalNode
   ,[OrganizationLevel]
   ,[JobTitle]
   ,[JobTitleHierarchical] = REPLICATE(N' ', OrganizationLevel * 4) + [JobTitle]
   ,[HireDate]
 FROM [AdventureWorks2012].[HumanResources].[Employee]
 --ORDER BY OrganizationNode
), CTE2 AS (
 SELECT *, ReversedLogical = REVERSE(LEFT(LogicalNode, LEN(LogicalNode) - 1))
 FROM CTE
), CTE3 AS (
 SELECT *, pos = CHARINDEX('/' COLLATE SQL_Latin1_General_CP1_CI_AS, ReversedLogical)
 FROM CTE2
), CTE4 AS (
 SELECT *, ParentNode = LEFT(LogicalNode, CASE WHEN pos = 0 THEN 0 ELSE LEN(LogicalNode) - pos END)
 FROM CTE3
)
SELECT BusinessEntityID AS EmployeeID, LoginID, mgr.ManagerID, JobTitle AS Title, HireDate
INTO HumanResources.EmployeeDemo 
FROM CTE4
OUTER APPLY (
 SELECT BusinessEntityID AS ManagerID
 FROM CTE
 WHERE CTE4.ParentNode = CTE.LogicalNode
) mgr

CREATE TABLE NewOrg
(
  OrgNode hierarchyid,
  EmployeeID int,
  LoginID nvarchar(50),
  ManagerID int
CONSTRAINT PK_NewOrg_OrgNode
  PRIMARY KEY CLUSTERED (OrgNode)
);
GO

CREATE TABLE #Children 
   (
    EmployeeID int,
    ManagerID int,
    Num int
);
GO

INSERT #Children (EmployeeID, ManagerID, Num)
SELECT EmployeeID, ManagerID,
  ROW_NUMBER() OVER (PARTITION BY ManagerID ORDER BY ManagerID) 
FROM EmployeeDemo
GO

WITH paths(path, EmployeeID) 
AS (
-- This section provides the value for the root of the hierarchy
SELECT hierarchyid::GetRoot() AS OrgNode, EmployeeID 
FROM #Children AS C 
WHERE ManagerID IS NULL 

UNION ALL 
-- This section provides values for all nodes except the root
SELECT 
CAST(p.path.ToString() + CAST(C.Num AS varchar(30)) + '/' AS hierarchyid), 
C.EmployeeID
FROM #Children AS C 
JOIN paths AS p 
   ON C.ManagerID = P.EmployeeID 
)
INSERT NewOrg (OrgNode, O.EmployeeID, O.LoginID, O.ManagerID)
SELECT P.path, O.EmployeeID, O.LoginID, O.ManagerID
FROM EmployeeDemo AS O 
JOIN Paths AS P 
   ON O.EmployeeID = P.EmployeeID
GO

ALTER TABLE NewOrg   
   ADD H_Level AS OrgNode.GetLevel() ;  

CREATE UNIQUE INDEX EmpBFInd   
   ON NewOrg(H_Level, OrgNode) ;  
GO  

ALTER TABLE NewOrg DROP COLUMN ManagerID ;  
GO  
 
--GetAncestor
-- Transact-SQL syntax child.GetAncestor ( n ) 
--Returns a hierarchyid representing the nth ancestor of this.


DECLARE @CurrentEmployee hierarchyid
SELECT @CurrentEmployee = OrganizationNode FROM [HumanResources].[Employee] 
WHERE LoginID = 'adventure-works\ken0'

SELECT OrganizationNode.ToString() AS Text_OrgNode, *
FROM [HumanResources].[Employee]
WHERE OrganizationNode.GetAncestor(2) = @CurrentEmployee;

--GetDescendant 
/*
parent.GetDescendant ( child1 , child2 ) 
Returns a child node of the parent.

Remarks
Returns one child node that is a descendant of the parent.
•If parent is NULL, returns NULL.
•If parent is not NULL, and both child1 and child2 are NULL, returns a child of parent.
•If parent and child1 are not NULL, and child2 is NULL, returns a child of parent greater than child1.
•If parent and child2 are not NULL and child1 is NULL, returns a child of parent less than child2.
•If parent, child1, and child2 are not NULL, returns a child of parent greater than child1 and less than child2.
•If child1 is not NULL and not a child of parent, an exception is raised.
•If child2 is not NULL and not a child of parent, an exception is raised.
•If child1 >= child2, an exception is raised.
*/
DECLARE @h hierarchyid = hierarchyid::GetRoot()
DECLARE @c hierarchyid = @h.GetDescendant(NULL, NULL)
SELECT @c.ToString()
DECLARE @c2 hierarchyid = @h.GetDescendant(@c, NULL)
SELECT @c2.ToString()
SET @c2 = @h.GetDescendant(@c, @c2)
SELECT @c2.ToString()
SET @c = @h.GetDescendant(@c, @c2)
SELECT @c.ToString()
SET @c2 = @h.GetDescendant(@c, @c2)
SELECT @c2.ToString()

--GetLevel 
--Returns an integer that represents the depth of the node this in the tree.
SELECT [OrganizationNode].GetLevel() FROM [HumanResources].[Employee] 

--GetRoot 
--hierarchyid::GetRoot ( ) 
--Returns the root of the hierarchy tree. GetRoot() is a static method.
SELECT * FROM [HumanResources].[Employee] 
WHERE [OrganizationNode]= hierarchyid::GetRoot()

--IsDescendantOf 
-- Transact-SQL syntax child. IsDescendantOf ( parent )
DECLARE @Manager hierarchyid
SELECT @Manager = [OrganizationNode] FROM HumanResources.Employee
WHERE LoginID = 'adventure-works\dylan0'

SELECT [OrganizationNode].ToString() Path,*
FROM HumanResources.Employee
WHERE [OrganizationNode].IsDescendantOf(@Manager) = 1
go

DECLARE @Manager hierarchyid, @Employee hierarchyid, @LoginID nvarchar(256)
SELECT @Manager = [OrganizationNode] FROM HumanResources.Employee
WHERE LoginID = 'adventure-works\terri0' ;

SELECT @Employee = [OrganizationNode], @LoginID = LoginID FROM HumanResources.Employee
WHERE LoginID = 'adventure-works\rob0'

IF @Employee.IsDescendantOf(@Manager) = 1
BEGIN
    PRINT 'LoginID ' + @LoginID + ' is a subordinate of the selected Manager.'
END
ELSE
BEGIN
    PRINT 'LoginID ' + @LoginID + ' is not a subordinate of the selected Manager.' ;
END

--Parse 
-- Transact-SQL syntax
-- hierarchyid::Parse ( input )
-- This is functionally equivalent to the following syntax 
-- which implicitly calls Parse():CAST ( input AS hierarchyid )
DECLARE @StringValue AS nvarchar(4000), @hierarchyidValue AS hierarchyid
SET @StringValue = '/1/1/3/'
SET @hierarchyidValue = 0x5ADE
SELECT hierarchyid::Parse(@StringValue) AS hierarchyidRepresentation,
@hierarchyidValue.ToString() AS StringRepresentation ;
GO

--Transact-SQL syntax node. GetReparentedValue ( oldRoot, newRoot )
--Returns a node whose path from the root is the path to newRoot, followed by the path from oldRoot to this.
--Can be used to modify the tree by moving nodes from oldRoot to newRoot. GetReparentedValue can be used to move a node of a hierarchy to a new location in the hierarchy. The hierarchyid data type represents but does not enforce the hierarchical structure. Users must ensure that the hierarchyid is appropriately structured for the new location. A unique index on the hierarchyid data type can help prevent duplicate entries. For an example of moving an entire subtree, see Hierarchical Data (SQL Server).

--Comparing two node locations
DECLARE @SubjectEmployee hierarchyid , @OldParent hierarchyid, @NewParent hierarchyid
SELECT @SubjectEmployee = [OrganizationNode] FROM HumanResources.Employee
  WHERE LoginID = 'adventure-works\gail0' ;
SELECT @OldParent = [OrganizationNode] FROM HumanResources.Employee
  WHERE LoginID = 'adventure-works\roberto0' ; -- who is /1/1/
SELECT @NewParent = [OrganizationNode] FROM HumanResources.Employee
  WHERE LoginID = 'adventure-works\wanida0' ; -- who is /2/3/

SELECT [OrganizationNode].ToString() AS Current_OrgNode_AS_Text, 
(@SubjectEmployee. GetReparentedValue(@OldParent, @NewParent) ).ToString() AS Proposed_OrgNode_AS_Text,
[OrganizationNode] AS Current_OrgNode,
@SubjectEmployee. GetReparentedValue(@OldParent, @NewParent) AS Proposed_OrgNode,
@OldParent.ToString() OldParent,@NewParent.ToString() NewParent,
*
FROM HumanResources.Employee
WHERE [OrganizationNode] = @SubjectEmployee ;
GO

--Updating a node to a new location
DECLARE @SubjectEmployee hierarchyid , @OldParent hierarchyid, @NewParent hierarchyid
SELECT @SubjectEmployee = OrgNode FROM HumanResources.EmployeeDemo
  WHERE LoginID = 'adventure-works\gail0' ;
SELECT @OldParent = OrgNode FROM HumanResources.EmployeeDemo
  WHERE LoginID = 'adventure-works\roberto0' ; -- who is /1/1/
SELECT @NewParent = OrgNode FROM HumanResources.EmployeeDemo
  WHERE LoginID = 'adventure-works\wanida0' ; -- who is /2/3/

SELECT OrgNode.ToString() AS Current_OrgNode_AS_Text, 
(@SubjectEmployee. GetReparentedValue(@OldParent, @NewParent) ).ToString() AS Proposed_OrgNode_AS_Text,
OrgNode AS Current_OrgNode,
@SubjectEmployee. GetReparentedValue(@OldParent, @NewParent) AS Proposed_OrgNode,
*
FROM HumanResources.EmployeeDemo
WHERE OrgNode = @SubjectEmployee ;
GO

--How to resolve the issue of a database that was in Recovery Pending mode
--Fundamentally , this error is closely correlated to that Forcibly deletion process  of File stream file which I do think no other solution except waiting to finish recovery mode and either wise it will end with :
--•The most optimistic   probability  that it will end up with Online mode …So it will be fine and no need for any further action  ( Just you have to wait for a longer time if log file was such huge)..
--•The most pessimistic probability that it will end up with suspect  mode  ..SO it will be need to run the below process of DB Repair  but bear in mind that data loss might be there;
--Stop SQL Server and remove transaction log file of this DB then restart again where DB should go with suspect mode ….If so you can run the below query


ALTER DATABASE [DB_Name] SET  SINGLE_USER WITH NO_WAIT
ALTER DATABASE [DB_Name] SET EMERGENCY;
DBCC checkdb ([DB_Name], REPAIR_ALLOW_DATA_LOSS  )
ALTER DATABASE [DB_Name] SET online;
ALTER DATABASE [DB_Name] SET  Multi_USER WITH NO_WAIT


------------------------------------------------------------------------------------------
--cursor demo
------------------------------------------------------------------------------------------
declare @code nvarchar(50)
declare @datetime datetimeoffset
declare @factor decimal(18,3)
declare RightsOfferingInfoCursor cursor for        
select top 10 code,datetime,factor from RightsOfferingInfo
open RightsOfferingInfoCursor                      
fetch next from RightsOfferingInfoCursor into @code,@datetime,@factor	           
while @@fetch_status=0						
begin
select @code,@datetime,@factor	
fetch next from RightsOfferingInfoCursor into @code,@datetime,@factor						
end
close RightsOfferingInfoCursor
deallocate RightsOfferingInfoCursor

---------------------------------------------------------------------------------------------------------
-- get table statistics rows
---replace the tablename when you use this script 
select id,object_name(id) as tableName,indid,rows,rowcnt from sys.sysindexes where id =object_id('tablename') and   indid in(0,1)