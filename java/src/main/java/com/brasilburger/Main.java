package com.brasilburger;

import java.sql.Connection;
import java.sql.SQLException;

public class Main {
    public static void main(String[] args) {
        try (Connection connection = DatabaseConnection.getConnection()) {
            if (connection != null) {
                System.out.println("Connexion à la base de données réussie !");
            }
        } catch (SQLException e) {
            System.err.println("Erreur de connexion à la base de données :");
            e.printStackTrace();
        }
    }
}