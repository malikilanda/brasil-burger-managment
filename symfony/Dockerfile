# Image PHP avec Apache
FROM php:8.2-apache

# Activer mod_rewrite (Symfony)
RUN a2enmod rewrite

# Installer dépendances système
RUN apt-get update && apt-get install -y \
    git \
    unzip \
    libpq-dev \
    && docker-php-ext-install pdo pdo_pgsql

# Installer Composer
COPY --from=composer:2 /usr/bin/composer /usr/bin/composer

# Définir le dossier de travail
WORKDIR /var/www/html

# Copier le projet
COPY . .

# Donner les droits
RUN chown -R www-data:www-data /var/www/html

# Installer les dépendances Symfony
RUN composer install --no-dev --optimize-autoloader

# Config Apache pour Symfony
RUN sed -i 's!/var/www/html!/var/www/html/public!g' /etc/apache2/sites-available/000-default.conf

# Exposer le port
EXPOSE 80

# Lancer Apache
CMD ["apache2-foreground"]
