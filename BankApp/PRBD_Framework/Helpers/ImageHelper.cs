using System;
using System.Diagnostics;
using System.IO;

namespace PRBD_Framework {
    /// <summary>
    /// Cette classe permet de gérer l'état d'un fichier (a priori une image, mais pas uniquement)
    /// lié à un champ dans la base de données, et notamment le fait qu'on peut charger une nouvelle
    /// version, puis décider d'annuler ou de confirmer le chargement (voir les différents états).
    /// </summary>
    public class ImageHelper {

        public enum ImageHelperState {
            Empty,              // Il n'y a pas de fichier associé
            LoadedAfterEmpty,   // Un fichier a été chargé alors qu'il n'y en avait pas et on est
                                // en attente de confirmation
            LoadedAfterSaved,   // Un fichier a été chargé alors qu'il y en avait déjà un et on est
                                // en attente de confirmation
            Saved,              // Le fichier chargé est confirmé et donc sauvé à l'emplacement choisi
                                // et avec le nom choisi
            ClearedAfterEmpty,  // Le fichier associé va être supprimé ; en attente de confirmation
            ClearedAfterSaved,  // Le fichier associé va être supprimé ; en attente de confirmation
        }

        public ImageHelperState State { get; private set; }

        private string basePath = null;                     // Le chemin de base où sont stockés les fichiers
        private string currentFile = null;                  // Le fichier sauvé courant
        private string tempFile = null;                     // Le fichier temporaire créé en cas de chargement en
                                                            // attendant la confirmation

        /// <summary>
        /// Le fichier qui doit être pris en compte dans l'état actuel. Si on est dans un des états temporaires
        /// c'est le fichier temporaire qui est retourné, sinon, c'est le fichier définitif ou null en fonction de l'état.
        /// </summary>
        public string CurrentFile {
            get {
                switch (State) {
                    case ImageHelperState.Empty:
                    case ImageHelperState.Saved:
                        return currentFile;
                    default:
                        return tempFile;
                }
            }
        }

        public bool IsTransitoryState {
            get => State == ImageHelperState.ClearedAfterEmpty ||
                State == ImageHelperState.ClearedAfterSaved ||
                State == ImageHelperState.LoadedAfterEmpty ||
                State == ImageHelperState.LoadedAfterSaved;
        }

        /// <summary>
        /// Crée un nouveau ImageHelper sur base d'un dossier où seront stockées les images
        /// et un éventuel fichier pré-existant à prendre en compte.
        /// </summary>
        /// <param name="basePath">Le chemin du dossier où sont stockées les images</param>
        /// <param name="currentFile">Le fichier image courant à partir duquel on initialise le gestionnaire d'état</param>
        public ImageHelper(string basePath, string currentFile = null) {
            this.basePath = basePath;
            this.currentFile = currentFile;
            State = currentFile == null ? ImageHelperState.Empty : ImageHelperState.Saved;
        }

        /// <summary>
        /// Demande le chargement d'une nouvelle image.
        /// </summary>
        /// <param name="newFilePath"></param>
        /// <returns>Le chemin relatif vers le fichier sauvegardé</returns>
        public string Load(string newFilePath) {
            if (newFilePath == null || !File.Exists(newFilePath)) {
                Debug.Assert(false, $"Load: bad file path '{newFilePath}'");
            }
            switch (State) {
                case ImageHelperState.ClearedAfterEmpty:
                case ImageHelperState.LoadedAfterEmpty:
                case ImageHelperState.Empty:
                    DeleteFile(tempFile);
                    CreateTempFile(newFilePath);
                    State = ImageHelperState.LoadedAfterEmpty;
                    break;
                case ImageHelperState.ClearedAfterSaved:
                case ImageHelperState.LoadedAfterSaved:
                case ImageHelperState.Saved:
                    DeleteFile(tempFile);
                    CreateTempFile(newFilePath);
                    State = ImageHelperState.LoadedAfterSaved;
                    break;
                default:
                    Console.WriteLine($"Load: bad action for state {State}");
                    break;
            }
            return CurrentFile;
        }

        /// <summary>
        /// Annule le chargement de l'image.
        /// </summary>
        /// <returns>Le chemin relatif vers le fichier sauvegardé, après annulation de l'opération en cours</returns>
        public string Cancel() {
            switch (State) {
                case ImageHelperState.LoadedAfterEmpty:
                    DeleteFile(tempFile);
                    State = ImageHelperState.Empty;
                    break;
                case ImageHelperState.LoadedAfterSaved:
                    DeleteFile(tempFile);
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.ClearedAfterSaved:
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.ClearedAfterEmpty:
                    State = ImageHelperState.Empty;
                    break;
                default:
                    // nothing to do
                    break;
            }
            return CurrentFile;
        }

        /// <summary>
        /// Confirme le chargement de l'image.
        /// On passe en paramètre le nom que doit avoir le fichier définitif, mais sans extension
        /// car on ne connait le nom définitif du fichier qu'au moment de la confirmation.
        /// On réutilise l'extension du fichier temporaire.
        /// </summary>
        /// <param name="targetFileWithoutExtension"></param>
        /// <returns>Le chemin relatif vers le fichier sauvegardé</returns>
        public string Confirm(string targetFileWithoutExtension) {
            switch (State) {
                case ImageHelperState.LoadedAfterEmpty:
                    RenameTempFile(targetFileWithoutExtension);
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.LoadedAfterSaved:
                    DeleteFile(currentFile);
                    RenameTempFile(targetFileWithoutExtension);
                    State = ImageHelperState.Saved;
                    break;
                case ImageHelperState.ClearedAfterSaved:
                    DeleteFile(currentFile);
                    currentFile = null;
                    State = ImageHelperState.Empty;
                    break;
                case ImageHelperState.ClearedAfterEmpty:
                case ImageHelperState.Empty:
                    State = ImageHelperState.Empty;
                    break;
                case ImageHelperState.Saved:
                    // nothing to do
                    break;
                default:
                    Console.WriteLine($"Confirm: bad action for state {State}");
                    break;
            }
            return CurrentFile;
        }

        /// <summary>
        /// Supprime l'image courante.
        /// </summary>
        /// <returns>null</returns>
        public string Clear() {
            switch (State) {
                case ImageHelperState.Saved:
                    tempFile = null;
                    State = ImageHelperState.ClearedAfterSaved;
                    break;
                case ImageHelperState.LoadedAfterEmpty:
                    DeleteFile(tempFile);
                    tempFile = null;
                    State = ImageHelperState.ClearedAfterEmpty;
                    break;
                case ImageHelperState.LoadedAfterSaved:
                    DeleteFile(tempFile);
                    tempFile = null;
                    State = ImageHelperState.ClearedAfterSaved;
                    break;
                default:
                    Console.WriteLine($"Clear: bad action for state {State}");
                    break;
            }
            return null;
        }

        private void CreateTempFile(string sourceFile) {
            var newExt = Path.GetExtension(sourceFile);
            var guid = Guid.NewGuid();
            tempFile = guid + newExt;
            File.Copy(sourceFile, Path.Combine(basePath, tempFile), overwrite: true);
        }

        private void DeleteFile(string file) {
            if (file == null) {
                return;
            }
            var path = Path.Combine(basePath, file);
            if (File.Exists(path)) {
                File.Delete(path);
            }
        }

        private void RenameTempFile(string targetFileWithoutExtension) {
            var oldPath = Path.Combine(basePath, tempFile);
            currentFile = targetFileWithoutExtension + Path.GetExtension(tempFile);
            var newPath = Path.Combine(basePath, currentFile);
            if (File.Exists(newPath)) {
                File.Delete(newPath);
            }
            File.Move(oldPath, newPath);
            UriToCachedImageConverter.ClearCache(newPath);
            tempFile = null;
        }
    }
}
