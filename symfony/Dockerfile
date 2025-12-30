# Image PHP + Apache
FROM php:8.3-apache

# Activer mod_rewrite (Symfony)
RUN a2enmod rewrite

# Dépendances système
RUN apt-get update && apt-get install -y \
    git unzip libpq-dev \
    && docker-php-ext-install pdo pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*

# Installer Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# Dossier de travail
WORKDIR /var/www/html

# Copier le projet
COPY . .

# Installer dépendances (prod)
RUN composer install --no-dev --optimize-autoloader

# Config Apache : document root = /public
RUN sed -i 's#/var/www/html#/var/www/html/public#g' /etc/apache2/sites-available/000-default.conf

# Permissions (cache/log)
RUN mkdir -p var/cache var/log && chown -R www-data:www-data var

# Port Render
EXPOSE 10000

# Apache écoute sur le port Render
RUN sed -i 's/Listen 80/Listen 10000/' /etc/apache2/ports.conf

CMD ["apache2-foreground"]
