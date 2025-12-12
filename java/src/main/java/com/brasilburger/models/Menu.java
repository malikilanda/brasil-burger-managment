package com.brasilburger.models;

public class Menu {
    private int id;
    private String name;
    private String image;
    private boolean archived;

    public Menu(String name, String image) {
        this.name = name;
        this.image = image;
        this.archived = false;
    }

    // Getters and Setters...
    public int getId() { return id; }
    public void setId(int id) { this.id = id; }
    public String getName() { return name; }
    public void setName(String name) { this.name = name; }
    public String getImage() { return image; }
    public void setImage(String image) { this.image = image; }
    public boolean isArchived() { return archived; }
    public void setArchived(boolean archived) { this.archived = archived; }

    @Override
    public String toString() {
        return "Menu #" + id + ": " + name;
    }
}
