CREATE VIEW 'V_GamesBeatenByYears' AS
SELECT DISTINCT(completedyear) 'Completed Year', 
(SELECT count(*) FROM Backlog where e.completedyear == completedyear and beaten = 1) 'Total', 
(SELECT count(*) FROM Backlog where e.completedyear == completedyear and max_time < 10) '<10',
(SELECT count(*) FROM Backlog where e.completedyear == completedyear and max_time > 10 and max_time < 20 ) '10-20',
(SELECT count(*) FROM Backlog where e.completedyear == completedyear and max_time > 30 and max_time < 50 ) '30-50',
(SELECT count(*) FROM Backlog where e.completedyear == completedyear and max_time > 50 and max_time < 100 ) '50-100',
(SELECT count(*) FROM Backlog where e.completedyear == completedyear and max_time > 100 ) '+100'
FROM Backlog e
where completedyear > 1970
order by completedyear desc