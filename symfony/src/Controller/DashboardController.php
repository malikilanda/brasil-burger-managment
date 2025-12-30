<?php

namespace App\Controller;

use App\Service\DashboardStats;
use Symfony\Bundle\FrameworkBundle\Controller\AbstractController;
use Symfony\Component\HttpFoundation\Response;
use Symfony\Component\Routing\Attribute\Route;

class DashboardController extends AbstractController
{
    #[Route('/', name: 'app_dashboard')]
    public function index(DashboardStats $statsService): Response
    {
        $stats = $statsService->getStats();

        return $this->render('dashboard/index.html.twig', [
            'stats' => $stats,
        ]);
    }
}
