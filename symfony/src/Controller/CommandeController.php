<?php

namespace App\Controller;

use Doctrine\DBAL\Connection;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class CommandeController extends AbstractController
{
    #[Route('/commande', name: 'app_commande')]
    public function index(Connection $conn): Response
    {
        $commandes = $conn->fetchAllAssociative("
            SELECT
                c.id,
                c.type,
                c.status,
                c.total_amount,
                c.created_at,
                c.user_id,
                COALESCE(u.firstname, '') || ' ' || COALESCE(u.lastname, '') AS client_nom,
                u.phone AS client_phone
            FROM commandes c
            LEFT JOIN users u ON u.id = c.user_id
            ORDER BY c.created_at DESC
        ");

        return $this->render('commande/index.html.twig', [
            'commandes' => $commandes,
        ]);
    }
}
