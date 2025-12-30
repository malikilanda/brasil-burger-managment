<?php

namespace App\Controller;

use Doctrine\DBAL\Connection;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class BurgerController extends AbstractController
{
    #[Route('/burger', name: 'app_burger')]
    public function index(Connection $conn): Response
    {
        $burgers = $conn->fetchAllAssociative("
            SELECT id, name, price, image, archived
            FROM burgers
            ORDER BY id ASC
        ");

        return $this->render('burger/index.html.twig', [
            'burgers' => $burgers,
        ]);
    }
}
