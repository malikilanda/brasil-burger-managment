package com.brasilburger.dao;

import com.brasilburger.DatabaseConnection;
import com.brasilburger.models.Burger;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

public class BurgerDAO {
    public void addBurger(Burger burger) {
        String sql = "INSERT INTO burgers (name, price, image) VALUES (?, ?, ?)";
        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement pstmt = conn.prepareStatement(sql)) {
            pstmt.setString(1, burger.getName());
            pstmt.setBigDecimal(2, burger.getPrice());
            pstmt.setString(3, burger.getImage());
            pstmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Erreur lors de l'ajout du burger : " + e.getMessage());
        }
    }

    public List<Burger> getAllBurgers() {
        List<Burger> burgers = new ArrayList<>();
        String sql = "SELECT id, name, price FROM burgers WHERE archived = FALSE ORDER BY id";
        try (Connection conn = DatabaseConnection.getConnection();
             Statement stmt = conn.createStatement();
             ResultSet rs = stmt.executeQuery(sql)) {
            while (rs.next()) {
                burgers.add(new Burger(
                    rs.getInt("id"),
                    rs.getString("name"),
                    rs.getBigDecimal("price")
                ));
            }
        } catch (SQLException e) {
            System.err.println("Erreur lors de la récupération des burgers : " + e.getMessage());
        }
        return burgers;
    }
}
