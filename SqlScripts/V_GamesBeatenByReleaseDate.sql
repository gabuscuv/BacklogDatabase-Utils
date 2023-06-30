CREATE VIEW 'V_GamesBeatenByReleaseDate' AS
SELECT DISTINCT(releaseyear), 
(SELECT count(*) FROM Backlog where e.releaseyear == releaseyear) 'Total',
(SELECT count(*) FROM Backlog where e.releaseyear == releaseyear and beaten = 1) 'Beaten',
(SELECT count(*) FROM Backlog where e.releaseyear == releaseyear and beaten = 0) 'Unbeaten'
FROM Backlog e
order by releaseyear desc