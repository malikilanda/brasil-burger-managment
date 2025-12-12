package com.brasilburger.models;

import java.math.BigDecimal;

public class Burger {
    private int id;
    private String name;
    private BigDecimal price;
    private String image;
    private boolean archived;

    public Burger(String name, BigDecimal price, String image) {
        this.name = name;
        this.price = price;
        this.image = image;
        this.archived = false;
    }

    public Burger(int id, String name, BigDecimal price) {
        this.id = id;
        this.name = name;
        this.price = price;
    }

    // Getters and Setters...
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public BigDecimal getPrice() { return price; }
    public void setPrice(BigDecimal price) { this.price = price; }
    public String getImage() { return image; }
    public void setImage(String image) { this.image = image; }
    public boolean isArchived() { return archived; }
    public void setArchived(boolean archived) { this.archived = archived; }

    @Override
    public String toString() {
        return "Burger #" + id + ": " + name + " - " + price + " FCFA";
    }
}
