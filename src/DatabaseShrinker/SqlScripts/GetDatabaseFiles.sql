Use [{0}]
select [name] as FileName, [type] as FileType from sys.database_files where [type] in (0,1)