﻿//-----------------------------------------------------------------------
// <copyright file="MondeTuiles.cs" company="Marco Lavoie">
// Marco Lavoie, 2010. Tous droits réservés
// 
// L'utilisation de ce matériel pédagogique (présentations, code source 
// et autres) avec ou sans modifications, est permise en autant que les 
// conditions suivantes soient respectées:
//
// 1. La diffusion du matériel doit se limiter à un intranet dont l'accès
//    est imité aux étudiants inscrits à un cours exploitant le dit 
//    matériel. IL EST STRICTEMENT INTERDIT DE DIFFUSER CE MATÉRIEL 
//    LIBREMENT SUR INTERNET.
// 2. La redistribution des présentations contenues dans le matériel 
//    pédagogique est autorisée uniquement en format Acrobat PDF et sous
//    restrictions stipulées à la condition #1. Le code source contenu 
//    dans le matériel pédagogique peut cependant être redistribué sous 
//    sa forme  originale, en autant que la condition #1 soit également 
//    respectée.
// 3. Le matériel diffusé doit contenir intégralement la mention de 
//    droits d'auteurs ci-dessus, la notice présente ainsi que la
//    décharge ci-dessous.
// 
// CE MATÉRIEL PÉDAGOGIQUE EST DISTRIBUÉ "TEL QUEL" PAR L'AUTEUR, SANS 
// AUCUNE GARANTIE EXPLICITE OU IMPLICITE. L'AUTEUR NE PEUT EN AUCUNE 
// CIRCONSTANCE ÊTRE TENU RESPONSABLE DE DOMMAGES DIRECTS, INDIRECTS, 
// CIRCONSTENTIELS OU EXEMPLAIRES. TOUTE VIOLATION DE DROITS D'AUTEUR 
// OCCASIONNÉ PAR L'UTILISATION DE CE MATÉRIEL PÉDAGOGIQUE EST PRIS EN 
// CHARGE PAR L'UTILISATEUR DU DIT MATÉRIEL.
// 
// En utilisant ce matériel pédagogique, vous acceptez implicitement les
// conditions et la décharge exprimés ci-dessus.
// </copyright>
//-----------------------------------------------------------------------

namespace ProjetFinale
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    /// <summary>
    /// Classe représentant un monde constitué de tuiles étant extraites d'une palette de tuiles.
    /// En plus de cette palette, l'instance de cette classe dispose aussi d'un tableau bi-dimensionnel
    /// indiquant l'index de chaque tuile (de la palette) constituant le monde.
    /// </summary>
    public class MondeTuiles : Monde
    {
        /// <summary>
        /// Palette de tuiles constituant le monde.
        /// </summary>
        private PaletteTuiles palette;

        /// <summary>
        /// Palette de tuiles permettant de gérer les collisions des sprites avec les tuiles.
        /// </summary>
        private PaletteTuiles paletteCollisions;

        /// <summary>
        /// Tableau d'index des tuiles du monde
        /// </summary>
        private int[,] mappeMonde;

        /// <summary>
        /// Constructeur paramétré.
        /// </summary>
        /// <param name="palette">Palette de tuiles à exploiter pour afficher le monde.</param>
        /// <param name="mappeMonde">Tableau d'index de tuiles pour chaque tuile du monde.</param>
        public MondeTuiles(PaletteTuiles palette, int[,] mappeMonde)
        {
            this.palette = palette;
            this.mappeMonde = mappeMonde;
        }

        /// <summary>
        /// Accesseur retournant la largeur du monde en pixels
        /// </summary>
        public override int Largeur
        {
            get { return this.mappeMonde.GetLength(1) * this.palette.LargeurTuile; }
        }

        /// <summary>
        /// Accesseur retournant la hauteur du monde en pixels
        /// </summary>
        public override int Hauteur
        {
            get { return this.mappeMonde.GetLength(0) * this.palette.HauteurTuile; }
        }

        /// <summary>
        /// Propriété (accesseur de paletteCollisions) retournant et changeant la palette de gestion des collisions
        /// de sprites avec les tuiles (peut être nul).
        /// </summary>
        /// <value>Palette de gestion des collisions de sprites avec les tuiles.</value>
        public PaletteTuiles PaletteCollisions
        {
            get { return this.paletteCollisions; }
            set { this.paletteCollisions = value; }
        }

        /// <summary>
        /// Affiche à l'écran la partie de la mappe monde visible par la camera.
        /// </summary>
        /// <param name="camera">Caméra à exploiter pour l'affichage.</param>
        /// <param name="spriteBatch">Gestionnaire d'affichage en batch aux périphériques.</param>
        public override void Draw(Camera camera, SpriteBatch spriteBatch)
        {
            // Initialiser le rectangle de destination aux dimensions d'une tuile
            Rectangle destRect = new Rectangle(0, 0, this.palette.LargeurTuile, this.palette.HauteurTuile);

            // Afficher une rangée à la fois
            for (int row = 0; row < this.mappeMonde.GetLength(0); row++)
            {
                for (int col = 0; col < this.mappeMonde.GetLength(1); col++)
                {
                    // Calculer la position de la tuile à l'écran
                    destRect.X = col * this.palette.LargeurTuile;
                    destRect.Y = row * this.palette.HauteurTuile;

                    // Afficher la tuile si elle est visible par la caméra
                    if (camera.EstVisible(destRect))
                    {
                        // Puisque le sprite est visible, déléguer à la palette de tuiles la tâche d'afficher
                        // la tuile courante.

                        // Décaler la destination en fonction de la caméra. Ceci correspond à transformer destRect 
                        // de coordonnées logiques (i.e. du monde) à des coordonnées physiques (i.e. de l'écran).
                        camera.Monde2Camera(ref destRect);

                        // Afficher la tuile courante
                        this.palette.Draw(this.mappeMonde[row, col], destRect, spriteBatch);
                    }
                }
            }
        }

        /// <summary>
        /// Retourne l'index de la tuile localisée à la position donnée (en coordonnées du monde).
        /// </summary>
        /// <param name="position">Position (en coordonnées du monde) à convertir en index de tuile.</param>
        /// <returns>Index de la tuile contenant la position fournie.</returns>
        public int MondeXY2TuileIdx(Vector2 position)
        {
            int row = (int)(position.Y / this.palette.HauteurTuile);
            int col = (int)(position.X / this.palette.LargeurTuile);

            return this.mappeMonde[row, col];
        }

        /// <summary>
        /// Fonction retournant la couleur du pixel aux coordonnées du monde (position) selon la 
        /// palette de collisions. La palette de collisions (si fournie) sert généralement à 
        /// indiquer les zones du monde où les sprites peuvent se déplacer. Similairement, elle
        /// peut aussi servir à indiquer le type de terrain à la position donnée.
        /// </summary>
        /// <param name="position">Position du pixel en coordonnées du monde.</param>
        /// <returns>Couleur dans la palette de collision aux coordonnées du monde fournies. Si
        /// aucune palette de collision n'est fournie, une exception est lancée.</returns>
        public override Color CouleurDeCollision(Vector2 position)
        {
            // S'assurer qu'on dispose d'une palette de gestion de collisions.
            if (this.paletteCollisions == null)
            {
                throw new NullReferenceException("Aucune palette de gestion de collisions fournie.");
            }

            // Extraire la couleur du pixel correspondant à la position donnée dans privTuilesCollisions.
            Color pixColor = this.paletteCollisions.CouleurDePixel(
                this.MondeXY2TuileIdx(position),
                (int)position.X % this.paletteCollisions.LargeurTuile,
                (int)position.Y % this.paletteCollisions.HauteurTuile);

            return pixColor;
        }
    }
}
