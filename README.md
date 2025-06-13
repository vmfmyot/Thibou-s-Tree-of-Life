
# Thibou's Tree of Life

3rd year University project for an Advanced Interactive Interfaces Programming class. Using [Kaggle's Tree of Life dataset](https://www.kaggle.com/datasets/konivat/tree-of-life?resource=download) of approximately 36000 species organized in nodes and links, we made a small phylogenetic tree with an educational purpose aimed towards children and teenagers alike.

## Authors

This small program was made by [Sidonie Minodier](https://github.com/shidowe) and [Victoria Myot](https://github.com/vmfmyot), computer science students at Université Paris-Saclay.
## User manual

The screen is split in 2 : on the left side, the **Tree Section**, which shows the current phylogenetic tree, and on the right the **Information Panel**, which shows informations relative to a selected node.

### **1. Tree Section :**

Each species is represented by a **Node**. When a species has over 5 children (descendants), it appears as a round button : a **cluster**.\
\
All nodes are buttons : clicking on them will **select** them and **show the information relevant to the represented species** in the Information Panel. When a species is **extinct**, its Node appears **black**; if not then **green**. For clusters, the more extinct descendants a species has, the darker it appears.\
\
The Tree Section is a ScrollableControl : a **horizontal scrollbar** is located at the bottom of the screen. The initial view shows the **complete tree**, starting from the original node **Life on Earth**, with its clusters closed off. When selecting a cluster, it will open and show a tree with the selected node as its new root.


### **2. Information Panel :**

At the top of the Information Panel is a **search bar**, with **filter checkboxes**. The user can simply type in a word and select a result. The selected node will appear as the root of a tree in the **Tree Section**.\
\
Whenever a node is **selected**, its **relevant information** will be displayed in the Information Panel : species name, extinct status, website link, phylesis type, path in the tree, number of children and number of extinct children. Selecting a species from the path or the name itself allows the user to travel back and forth through the phylogenetic tree.\
\
If the nodes contains a **valid link** to the [tolweb.org](tolweb.org) website, **clicking on the link** will open a window to the aforementioned link directly in the browser, in which the user can access additional information about the species.\
\
On the bottom left side of the Information Panel, a few **interesting species** are displayed : rat, owl, giraffe, mockingbird and whale. **Clicking on a name** selects the relevant node and shows the tree and the information relevant to the species. As the dataset mostly contains bacteria and viruses and the application is **aimed towards a younger audience**, we decided to show well-known animal species in order to stimulate their interest into science and evolution.
## Additional comments

While struggling to keep the interface as simple as possible, we wanted to add a fun touch with a reference to a well-known and beloved videogame, *Animal Crossing*. Blathers, also known as **Thibou** in french, is a museum curator : in this project, he teaches children about science and evolution.
