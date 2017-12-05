# TODO create copy of configuration
### Apache

Add separate template for fastcgi env variables

```text
# Get source config from conf/original/

# Change paths in httpd.conf
ServerRoot
DocumentRoot
<Directory "c:/Apache24/htdocs">
<IfModule alias_module>
    ScriptAlias
</IfModule>
<Directory "c:/Apache24/cgi-bin">

# Change paths in conf/extra/httpd-dav.conf
DavLockDB "c:/Apache24/var/DavLock"
Alias /uploads "c:/Apache24/uploads"
<Directory "c:/Apache24/uploads">

# Change paths in conf/extra/httpd-errordoc.conf
Alias /error/ "c:/Apache24/error/"
<Directory "c:/Apache24/error">

# Change paths in conf/extra/httpd-ssl.conf
SSLSessionCache
# All path in:
<VirtualHost _default_:443>
```
And add default domain in httpd-vhosts.conf
### PHP
Get original from php.ini-development
```text
max_execution_time = 60
max_input_vars = 10000
memory_limit = 1024M
error_log = /default_path_to.log
post_max_size = 512M
default_charset = "UTF-8"
upload_max_filesize = 512M
session.save_path = "/tmp"
soap.wsdl_cache_dir="/tmp"
```