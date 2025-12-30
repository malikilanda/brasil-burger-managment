<?php

namespace App\Controller;

use Doctrine\DBAL\Connection;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class MenuController extends AbstractController
{
    #[Route('/menu', name: 'app_menu')]
    public function index(Connection $conn): Response
    {
        $menus = $conn->fetchAllAssociative("
            SELECT id, name, price, image, archived
            FROM menus
            ORDER BY id ASC
        ");

        return $this->render('menu/index.html.twig', [
            'menus' => $menus,
        ]);
    }
}
