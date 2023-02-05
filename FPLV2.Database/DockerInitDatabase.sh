i=0
echo "Begin Script Execution"
while [ $i -le 100 ];
do
   /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d master -Q "CREATE DATABASE $2"
   /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d master -Q "USE $2"
   if [ $? -eq 0 ]
   then
      echo "Database Created"
      break
   else
      echo "SQL Server not ready yet"
      sleep 1
   fi
   i=$(( i + 1 ))
done

echo "Creating Tables"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -i CreateTables.sql
echo "Creating Stored Procedures"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -i CreateStoredProcedures.sql

sleep infinity