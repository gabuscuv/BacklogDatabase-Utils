CREATE VIEW "V_Backlog_VR" AS SELECT *
FROM Backlog e
where (e.plataform like '%VR%' or e.plataform like '%Oculus%')
 and e.beaten == 0 