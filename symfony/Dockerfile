# ====== STAGE 1: build vendors with PHP 8.3 + Composer ======
FROM php:8.3-cli AS vendor

WORKDIR /app

RUN apt-get update && apt-get install -y \
    git unzip libzip-dev libicu-dev libpq-dev curl \
    && docker-php-ext-install intl zip pdo_pgsql \
    && rm -rf /var/lib/apt/lists/*

COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# deps d'abord pour cache
COPY composer.json composer.lock symfony.lock* ./

# IMPORTANT: pas de scripts ici (bin/console n'existe pas encore)
RUN composer install \
    --no-dev \
    --no-interaction \
    --no-progress \
    --prefer-dist \
    --optimize-autoloader \
    --no-scripts

# ====== STAGE 2: runtime Apache + PHP 8.3 ======
FROM php:8.3-apache

RUN apt-get update && apt-get install -y \
    unzip libpq-dev libicu-dev libzip-dev \
    && docker-php-ext-install pdo_pgsql intl zip \
    && a2enmod rewrite \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /var/www/html

# Autoriser .htaccess dans /public (Symfony routing)
RUN printf '%s\n' \
'<Directory /var/www/html/public>' \
'    AllowOverride All' \
'    Require all granted' \
'</Directory>' \
> /etc/apache2/conf-available/symfony.conf \
&& a2enconf symfony


COPY . .
COPY --from=vendor /app/vendor ./vendor

ENV APACHE_DOCUMENT_ROOT=/var/www/html/public
RUN sed -ri -e 's!/var/www/html!/var/www/html/public!g' /etc/apache2/sites-available/*.conf \
 && sed -ri -e 's!/var/www/!/var/www/html/public!g' /etc/apache2/apache2.conf /etc/apache2/conf-available/*.conf

RUN mkdir -p var/cache var/log && chown -R www-data:www-data var

ENV APP_ENV=prod
ENV APP_DEBUG=0

# optionnel: warmup cache (ne casse pas le build)
RUN php bin/console cache:clear --env=prod --no-debug || true

EXPOSE 80
