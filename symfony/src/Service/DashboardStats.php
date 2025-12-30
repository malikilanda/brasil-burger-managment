<?php

namespace App\Service;

use Doctrine\DBAL\Connection;

class DashboardStats
{
    public function __construct(private Connection $conn)
    {
    }

    public function getStats(): array
    {
        // Total commandes
        $totalOrders = (int) $this->conn->fetchOne("
            SELECT COUNT(*) FROM commandes
        ");

        // Commandes en attente / en cours (selon ce que tu utilises dans status)
        // Ici on supporte plusieurs valeurs possibles: 'attente', 'en_cours', 'pending', 'processing'
        $pendingOrders = (int) $this->conn->fetchOne("
            SELECT COUNT(*)
            FROM commandes
            WHERE status IN ('attente','en_cours','pending','processing')
        ");

        // Commandes terminées
        // Ici on supporte: 'terminee', 'livree', 'done', 'completed'
        $doneOrders = (int) $this->conn->fetchOne("
            SELECT COUNT(*)
            FROM commandes
            WHERE status IN ('terminee','livree','done','completed')
        ");

        // Totaux “produits” (si tes tables existent bien)
        $totalBurgers = (int) $this->conn->fetchOne("SELECT COUNT(*) FROM burgers");
        $totalMenus = (int) $this->conn->fetchOne("SELECT COUNT(*) FROM menus");
        $totalComplements = (int) $this->conn->fetchOne("SELECT COUNT(*) FROM complements");

        // Recettes : somme total_amount (NULL -> 0)
        $recettes = (int) $this->conn->fetchOne("
            SELECT COALESCE(SUM(total_amount), 0)
            FROM commandes
        ");

        // Dernières commandes (dashboard table)
        // On récupère le client via users.firstname/lastname.
        // Quartier/zone: si tu veux les afficher on left join quartiers/zones (si ça existe).
        $recentOrders = $this->conn->fetchAllAssociative("
            SELECT
                c.id,
                c.type,
                c.status,
                c.total_amount,
                c.created_at,
                u.firstname AS client_firstname,
                u.lastname  AS client_lastname,
                u.phone     AS client_phone
            FROM commandes c
            LEFT JOIN users u ON u.id = c.user_id
            ORDER BY c.created_at DESC
            LIMIT 10
        ");

        return [
            'totalOrders'      => $totalOrders,
            'pendingOrders'    => $pendingOrders,
            'doneOrders'       => $doneOrders,
            'totalBurgers'     => $totalBurgers,
            'totalMenus'       => $totalMenus,
            'totalComplements' => $totalComplements,
            'recettes'         => $recettes,
            'recentOrders'     => $recentOrders,
        ];
    }
}
