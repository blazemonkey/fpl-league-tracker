i=0
echo "Begin Script Execution"
while [ $i -le 100 ];
do
   DATABASE_EXISTS=$(/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d master -h -1 -W -Q "set nocount on;select count(1) from sys.databases where name='$2'")
   if [ $? -eq 0 ]
   then
      if [ $DATABASE_EXISTS -eq 0 ]
      then
         echo "Database Created"
         break
      else
	     echo "Database Already Exists"
         CURRENT_VERSION=$(/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -h -1 -W -Q "set nocount on;exec GetVersion")
         if [ $CURRENT_VERSION -ge 100 ]
         then
            break
         else
            sleep 5
         fi
      fi
   else
      echo "SQL Server not ready yet"
      sleep 1
   fi
   i=$(( i + 1 ))
done

if [ $DATABASE_EXISTS -eq 0 ]
then
   echo "Creating Tables"
   /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -i CreateTables.sql
   echo "Creating Stored Procedures"
   /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -i Programmability/CreateStoredProcedures.sql
   echo "Insert Initial Data"
   /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -i InitialData.sql
else
   echo "Checking Database Upgrade"
   for SCRIPT in $(ls Upgrades/*.sql | sort -n)
   do
      VERSION=$(basename $SCRIPT | cut -d '.' -f1)
      if [ $VERSION -gt $CURRENT_VERSION ]
      then
        echo "Running $VERSION Upgrade"
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -i $SCRIPT
      fi	
   done   
fi

echo "Database Check Complete"
sleep infinity