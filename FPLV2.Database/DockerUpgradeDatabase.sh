CURRENT_VERSION=/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $1 -d $2 -Q "EXEC GetVersion"
echo "Current Version: $CURRENT_VERSION"

for SCRIPT in $(ls Upgrades/*.sql | sort -n)
do
  VERSION=$(basename $SCRIPT | cut -d '.' -f1)
  if [ $VERSION -gt $CURRENT_VERSION ]
  then
    echo "Running upgrade script $SCRIPT"

  fi
done