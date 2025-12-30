# ====== STAGE 1: dependencies (Composer) ======
FROM composer:2 AS vendor

WORKDIR /app

# Copie uniquement les fichiers nécessaires au install (cache Docker)
COPY composer.json composer.lock symfony.lock* ./

# Installer les vendors (sans dev)
RUN composer install \
    --no-dev \
    --no-interaction \
    --no-ansi \
    --no-progress \
    --prefer-dist \
    --optimize-autoloader

# ====== STAGE 2: runtime (PHP + Apache) ======
FROM php:8.3-apache

# Paquets + extensions nécessaires (Postgres + intl + zip)
RUN apt-get update && apt-get install -y \
    git unzip libpq-dev libicu-dev libzip-dev \
    && docker-php-ext-install pdo_pgsql intl zip \
    && a2enmod rewrite \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /var/www/html

# Copier l'app Symfony
COPY . .

# Copier vendor depuis le stage Composer
COPY --from=vendor /app/vendor ./vendor

# Apache doit pointer vers /public
ENV APACHE_DOCUMENT_ROOT=/var/www/html/public
RUN sed -ri -e 's!/var/www/html!/var/www/html/public!g' /etc/apache2/sites-available/*.conf \
    && sed -ri -e 's!/var/www/!/var/www/html/public!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

# Permissions pour cache/log
RUN mkdir -p var/cache var/log \
    && chown -R www-data:www-data var

# Symfony en prod
ENV APP_ENV=prod
ENV APP_DEBUG=0

EXPOSE 80
