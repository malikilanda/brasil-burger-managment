-- =============================================
--        DATABASE : brasil_burger (Minimal)
-- =============================================

-- À exécuter seulement si tu veux créer une DB :
-- CREATE DATABASE brasil_burger;
-- \c brasil_burger;

-- ================================
-- TABLE : users (client / gestionnaire / livreur)
-- Gère les clients, le gestionnaire et les livreurs.
-- ================================
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    firstname VARCHAR(100) NOT NULL,
    lastname VARCHAR(100) NOT NULL,
    phone VARCHAR(20) UNIQUE NOT NULL,
    email VARCHAR(150),
    password VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL CHECK (role IN ('CLIENT','GESTIONNAIRE','LIVREUR')),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ================================
-- TABLE : burgers
-- ================================
CREATE TABLE burgers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    image VARCHAR(255),
    archived BOOLEAN DEFAULT FALSE -- Ajout/Modification/Archivage [cite: 6]
);

-- ================================
-- TABLE : complements
-- (Utilisé pour les frites et boissons des menus ou en suppléments)
-- ================================
CREATE TABLE complements (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    price DECIMAL(10,2) NOT NULL,
    type VARCHAR(20) NOT NULL CHECK (type IN ('BOISSON','FRITES')),
    image VARCHAR(255),
    archived BOOLEAN DEFAULT FALSE -- Ajout/Modification/Archivage [cite: 6]
);

-- ================================
-- TABLE : menus
-- ================================
CREATE TABLE menus (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    image VARCHAR(255),
    archived BOOLEAN DEFAULT FALSE -- Ajout/Modification/Archivage [cite: 6]
);

-- ================================
-- TABLE : menu_composition
-- (Un menu = Burger + Boisson + Frites) [cite: 5]
-- ================================
CREATE TABLE menu_composition (
    id SERIAL PRIMARY KEY,
    menu_id INT NOT NULL REFERENCES menus(id) ON DELETE CASCADE,
    burger_id INT NOT NULL REFERENCES burgers(id),
    boisson_id INT NOT NULL REFERENCES complements(id),
    frites_id INT NOT NULL REFERENCES complements(id),
    UNIQUE(menu_id)
);

-- ================================
-- TABLE : zones
-- (Regrouper les commandes par zone) [cite: 14]
-- ================================
CREATE TABLE zones (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    delivery_price DECIMAL(10,2) NOT NULL -- La zone a un prix [cite: 15]
);

-- ================================
-- TABLE : quartiers
-- (Une zone couvre des quartiers) [cite: 15]
-- ================================
CREATE TABLE quartiers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    zone_id INT NOT NULL REFERENCES zones(id) ON DELETE CASCADE
);

-- ================================
-- TABLE : commandes
-- ================================
CREATE TABLE commandes (
    id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES users(id), -- Client qui commande [cite: 9]
    type VARCHAR(20) NOT NULL CHECK (type IN ('SUR_PLACE','A_EMPORTER','LIVRAISON')), -- Type de consommation [cite: 10]
    quartier_id INT REFERENCES quartiers(id), -- Pour la livraison
    status VARCHAR(20) DEFAULT 'EN_COURS' -- Pour le suivi des commandes [cite: 9, 13]
        CHECK (status IN ('EN_COURS','VALIDEE','TERMINEE','ANNULEE')),
    total_amount DECIMAL(10,2), -- Nécessaire pour les recettes/statistiques
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP -- Pour les filtres par date [cite: 21] et les stats
);

-- ================================
-- TABLE : commande_burgers
-- (Détail des burgers commandés seuls)
-- ================================
CREATE TABLE commande_burgers (
    id SERIAL PRIMARY KEY,
    commande_id INT NOT NULL REFERENCES commandes(id) ON DELETE CASCADE,
    burger_id INT NOT NULL REFERENCES burgers(id),
    quantity INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(10,2) NOT NULL -- Figer le prix au moment de la commande
);

-- ================================
-- TABLE : commande_menus
-- (Détail des menus commandés)
-- ================================
CREATE TABLE commande_menus (
    id SERIAL PRIMARY KEY,
    commande_id INT NOT NULL REFERENCES commandes(id) ON DELETE CASCADE,
    menu_id INT NOT NULL REFERENCES menus(id),
    quantity INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(10,2) NOT NULL -- Figer le prix au moment de la commande
);

-- ================================
-- TABLE : commande_complements
-- (Compléments supplémentaires proposés / ajoutés) [cite: 5, 10]
-- ================================
CREATE TABLE commande_complements (
    id SERIAL PRIMARY KEY,
    commande_id INT NOT NULL REFERENCES commandes(id) ON DELETE CASCADE,
    complement_id INT NOT NULL REFERENCES complements(id),
    quantity INT NOT NULL DEFAULT 1,
    unit_price DECIMAL(10,2) NOT NULL
);

-- ================================
-- TABLE : paiements
-- (La commande doit être payée pour être valide) [cite: 11]
-- ================================
CREATE TABLE paiements (
    id SERIAL PRIMARY KEY,
    commande_id INT UNIQUE NOT NULL REFERENCES commandes(id) ON DELETE CASCADE, -- Une commande est payée une seule fois [cite: 19]
    montant DECIMAL(10,2) NOT NULL,
    mode VARCHAR(10) NOT NULL CHECK (mode IN ('WAVE','OM')), -- Paiement par Wave ou OM [cite: 16]
    date_paiement TIMESTAMP DEFAULT CURRENT_TIMESTAMP -- Date et montant enregistrés [cite: 15]
);

-- ================================
-- TABLE : livraisons
-- (Affectation du livreur et suivi des livraisons) [cite: 14]
-- ================================
CREATE TABLE livraisons (
    id SERIAL PRIMARY KEY,
    commande_id INT UNIQUE NOT NULL REFERENCES commandes(id) ON DELETE CASCADE,
    livreur_id INT NOT NULL REFERENCES users(id),
    zone_id INT NOT NULL REFERENCES zones(id),
    status VARCHAR(20) DEFAULT 'EN_ATTENTE'
        CHECK (status IN ('EN_ATTENTE','EN_COURS','LIVREE')),
    date_affectation TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);