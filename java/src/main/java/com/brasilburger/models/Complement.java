package com.brasilburger.models;

import java.math.BigDecimal;

public class Complement {
    private int id;
    private String name;
    private BigDecimal price;
    private String type; // 'BOISSON' or 'FRITES'
    private String image;
    private boolean archived;

    public Complement(int id, String name, String type) {
        this.id = id;
        this.name = name;
        this.type = type;
    }

    public Complement(String name, BigDecimal price, String type, String image) {
        this.name = name;
        this.price = price;
        this.type = type;
        this.image = image;
        this.archived = false;
    }

    // Getters and Setters...
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public BigDecimal getPrice() { return price; }
    public void setPrice(BigDecimal price) { this.price = price; }
    public String getType() { return type; }
    public void setType(String type) { this.type = type; }
    public String getImage() { return image; }
    public void setImage(String image) { this.image = image; }
    public boolean isArchived() { return archived; }
    public void setArchived(boolean archived) { this.archived = archived; }

    @Override
    public String toString() {
        return "Complement #" + id + ": " + name + " (" + type + ")";
    }
}
