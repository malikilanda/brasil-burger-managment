package com.brasilburger;

import com.brasilburger.dao.BurgerDAO;
import com.brasilburger.dao.ComplementDAO;
import com.brasilburger.dao.MenuDAO;
import com.brasilburger.models.Burger;
import com.brasilburger.models.Complement;
import com.brasilburger.models.Menu;
import com.googlecode.lanterna.TerminalPosition;
import com.googlecode.lanterna.TextColor;
import com.googlecode.lanterna.graphics.TextGraphics;
import com.googlecode.lanterna.input.KeyStroke;
import com.googlecode.lanterna.input.KeyType;
import com.googlecode.lanterna.screen.Screen;
import com.googlecode.lanterna.screen.TerminalScreen;
import com.googlecode.lanterna.terminal.DefaultTerminalFactory;
import com.googlecode.lanterna.terminal.Terminal;

import java.io.IOException;
import java.math.BigDecimal;
import java.util.List;
import java.util.function.Function;

public class Main {

    private static final BurgerDAO burgerDAO = new BurgerDAO();
    private static final ComplementDAO complementDAO = new ComplementDAO();
    private static final MenuDAO menuDAO = new MenuDAO();

    public static void main(String[] args) throws IOException {
        DefaultTerminalFactory terminalFactory = new DefaultTerminalFactory();
        try (Terminal terminal = terminalFactory.createTerminal();
             Screen screen = new TerminalScreen(terminal)) {
            screen.startScreen();
            while (true) {
                List<String> options = List.of("Ajouter un Burger", "Ajouter un Complément", "Créer un Menu", "Quitter");
                int choice = promptSelection(screen, "--- Gestion Brasil Burger ---", options, Function.identity());
                if (choice == -1 || choice == 3) break;

                switch (choice) {
                    case 0: addBurger(screen); break;
                    case 1: addComplement(screen); break;
                    case 2: createMenu(screen); break;
                }
                screen.clear();
                TextGraphics g = screen.newTextGraphics();
                g.putString(0,0, "Opération terminée. Appuyez sur une touche pour revenir au menu...");
                screen.refresh();
                screen.readInput();
            }
        }
    }

    private static void addBurger(Screen screen) throws IOException {
        screen.clear();
        String name = promptTextInput(screen, "Nom du burger: ", 0);
        String priceStr = promptTextInput(screen, "Prix: ", 1);
        String image = promptTextInput(screen, "URL de l'image: ", 2);
        try {
            burgerDAO.addBurger(new Burger(name, new BigDecimal(priceStr), image));
            screen.newTextGraphics().putString(0, 4, "Burger '" + name + "' ajouté !");
        } catch (Exception e) {
            screen.newTextGraphics().putString(0, 4, "Erreur: " + e.getMessage());
        }
        screen.refresh();
    }

    private static void addComplement(Screen screen) throws IOException {
        screen.clear();
        String name = promptTextInput(screen, "Nom du complément: ", 0);
        String priceStr = promptTextInput(screen, "Prix: ", 1);
        List<String> types = List.of("BOISSON", "FRITES");
        int typeChoice = promptSelection(screen, "Type du complément:", types, Function.identity());
        if (typeChoice == -1) return;
        String type = types.get(typeChoice);
        String image = promptTextInput(screen, "URL de l'image: ", 2);
        try {
            complementDAO.addComplement(new Complement(name, new BigDecimal(priceStr), type, image));
            screen.newTextGraphics().putString(0, 5, "Complément '" + name + "' ajouté !");
        } catch (Exception e) {
            screen.newTextGraphics().putString(0, 5, "Erreur: " + e.getMessage());
        }
        screen.refresh();
    }

    private static void createMenu(Screen screen) throws IOException {
        screen.clear();
        String name = promptTextInput(screen, "Nom du menu: ", 0);
        String image = promptTextInput(screen, "URL de l'image: ", 1);

        List<Burger> burgers = burgerDAO.getAllBurgers();
        int burgerIndex = promptSelection(screen, "Choisissez un burger:", burgers, Burger::toString);
        if (burgerIndex == -1) return;
        Burger selectedBurger = burgers.get(burgerIndex);

        List<Complement> boissons = complementDAO.getComplementsByType("BOISSON");
        int boissonIndex = promptSelection(screen, "Choisissez une boisson:", boissons, Complement::toString);
        if (boissonIndex == -1) return;
        Complement selectedBoisson = boissons.get(boissonIndex);

        List<Complement> frites = complementDAO.getComplementsByType("FRITES");
        int fritesIndex = promptSelection(screen, "Choisissez les frites:", frites, Complement::toString);
        if (fritesIndex == -1) return;
        Complement selectedFrites = frites.get(fritesIndex);

        int confirm = promptSelection(screen, "Confirmer la création ?", List.of("Oui", "Non"), Function.identity());

        screen.clear();
        if (confirm == 0) { // "Oui"
            menuDAO.createMenu(new Menu(name, image), selectedBurger.getId(), selectedBoisson.getId(), selectedFrites.getId());
            screen.newTextGraphics().putString(0, 0, "Menu '" + name + "' créé !");
        } else {
            screen.newTextGraphics().putString(0, 0, "Création annulée.");
        }
        screen.refresh();
    }

    private static <T> int promptSelection(Screen screen, String title, List<T> options, Function<T, String> formatter) throws
   IOException {
        if (options == null || options.isEmpty()) {
            screen.newTextGraphics().putString(0,0, "Erreur: Aucun élément à sélectionner.");
            screen.refresh();
            screen.readInput();
            return -1;
        }
        int selected = 0;
        while (true) {
            screen.clear();
            TextGraphics g = screen.newTextGraphics();
            g.putString(0, 0, title);
            for (int i = 0; i < options.size(); i++) {
                g.setForegroundColor(i == selected ? TextColor.ANSI.CYAN : TextColor.ANSI.DEFAULT);
                g.putString(0, i + 2, (i == selected ? "> " : "  ") + formatter.apply(options.get(i)));
            }
            screen.refresh();
            KeyStroke key = screen.readInput();
            if (key.getKeyType() == KeyType.ArrowDown) selected = (selected + 1) % options.size();
            else if (key.getKeyType() == KeyType.ArrowUp) selected = (selected - 1 + options.size()) % options.size();
            else if (key.getKeyType() == KeyType.Enter) return selected;
            else if (key.getKeyType() == KeyType.Escape) return -1;
        }
    }

    private static String promptTextInput(Screen screen, String title, int row) throws IOException {
        TextGraphics g = screen.newTextGraphics();
        g.putString(0, row, title);
        screen.refresh();
        StringBuilder input = new StringBuilder();
        int colOffset = title.length();
        while(true) {
            screen.setCursorPosition(new TerminalPosition(colOffset + input.length(), row));
            screen.refresh();
            KeyStroke key = screen.readInput();
            if(key.getKeyType() == KeyType.Enter) break;
            else if (key.getKeyType() == KeyType.Backspace && input.length() > 0) input.deleteCharAt(input.length() - 1);
            else if (key.getKeyType() == KeyType.Character) input.append(key.getCharacter());
            g.putString(colOffset, row, " ".repeat(50));
            g.putString(colOffset, row, input.toString());
        }
        return input.toString();
    }
}
