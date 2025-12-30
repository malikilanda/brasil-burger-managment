# ====== STAGE 1: build vendors with PHP 8.3 + Composer ======
FROM php:8.3-cli AS vendor

WORKDIR /app

# deps utiles pour composer
RUN apt-get update && apt-get install -y \
    git unzip libzip-dev libicu-dev libpq-dev curl \
    && docker-php-ext-install intl zip pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*

# installer Composer (officiel)
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# copier uniquement les fichiers de deps d'abord (cache Docker)
COPY composer.json composer.lock symfony.lock* ./

# installer vendors (prod)
RUN composer install \
    --no-dev \
    --no-interaction \
    --no-progress \
    --prefer-dist \
    --optimize-autoloader

# ====== STAGE 2: runtime Apache + PHP 8.3 ======
FROM php:8.3-apache

RUN apt-get update && apt-get install -y \
    unzip libpq-dev libicu-dev libzip-dev \
    && docker-php-ext-install pdo_pgsql intl zip \
    && a2enmod rewrite \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /var/www/html

# copier ton code
COPY . .

# copier vendor depuis stage 1
COPY --from=vendor /app/vendor ./vendor

# Apache -> /public
ENV APACHE_DOCUMENT_ROOT=/var/www/html/public
RUN sed -ri -e 's!/var/www/html!/var/www/html/public!g' /etc/apache2/sites-available/*.conf \
 && sed -ri -e 's!/var/www/!/var/www/html/public!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

RUN mkdir -p var/cache var/log && chown -R www-data:www-data var

ENV APP_ENV=prod
ENV APP_DEBUG=0

EXPOSE 80
