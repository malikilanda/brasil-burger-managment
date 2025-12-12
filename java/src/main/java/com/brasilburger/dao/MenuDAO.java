package com.brasilburger.dao;

import com.brasilburger.DatabaseConnection;
import com.brasilburger.models.Menu;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.SQLException;

public class MenuDAO {
    public void createMenu(Menu menu, int burgerId, int boissonId, int fritesId) {
        // This is a placeholder implementation.
        System.out.println("Creating menu: " + menu.getName());
        System.out.println("Burger ID: " + burgerId);
        System.out.println("Boisson ID: " + boissonId);
        System.out.println("Frites ID: " + fritesId);
    }
}
