package com.brasilburger.dao;

import com.brasilburger.DatabaseConnection;
import com.brasilburger.models.Complement;
import java.sql.*;
import java.util.ArrayList;
import java.util.List;

public class ComplementDAO {
    public void addComplement(Complement complement) {
        String sql = "INSERT INTO complements (name, price, type, image) VALUES (?, ?, ?, ?)";
        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement pstmt = conn.prepareStatement(sql)) {
            pstmt.setString(1, complement.getName());
            pstmt.setBigDecimal(2, complement.getPrice());
            pstmt.setString(3, complement.getType());
            pstmt.setString(4, complement.getImage());
            pstmt.executeUpdate();
        } catch (SQLException e) {
            System.err.println("Erreur lors de l'ajout du complément : " + e.getMessage());
        }
    }

    public List<Complement> getComplementsByType(String type) {
        List<Complement> complements = new ArrayList<>();
        String sql = "SELECT id, name, type FROM complements WHERE type = ? AND archived = FALSE ORDER BY id";
        try (Connection conn = DatabaseConnection.getConnection();
             PreparedStatement pstmt = conn.prepareStatement(sql)) {
            pstmt.setString(1, type);
            ResultSet rs = pstmt.executeQuery();
            while (rs.next()) {
                complements.add(new Complement(
                    rs.getInt("id"),
                    rs.getString("name"),
                    rs.getString("type")
                ));
            }
        } catch (SQLException e) {
            System.err.println("Erreur lors de la récupération des compléments : " + e.getMessage());
        }
        return complements;
    }
}
